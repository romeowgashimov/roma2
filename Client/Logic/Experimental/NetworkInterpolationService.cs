using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Godot;
using ROMA2.Client.Logic.Extensions;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using static ROMA2.Shared.Interfaces.INetworkTransform;

namespace ROMA2.Client.Logic.Experimental;

public sealed unsafe class NetworkInterpolationService : IDisposable
{
    // 22 плоских канала на каждый активный объект:
    // t (1) + lhsPos (3) + rhsPos (3) + lhsRot (4) + rhsRot (4) + outPos (3) + outRot (4) = 22
    private const int FloatChannelsCount = 22;
    private const nuint Alignment = 32; // Выравнивание под 256-битные регистры AVX

    private readonly List<INetworkTransformCentralized> _targets = new();
    private readonly bool _isAvxSupported;

    private byte* _rawMemory;
    private int _capacity; // Всегда кратно 8

    // Указатели на срезы внутри единого выровненного блока памяти
    private int* _activeIndices;
    private float* _t;

    private float* _lhsPosX, _lhsPosY, _lhsPosZ;
    private float* _rhsPosX, _rhsPosY, _rhsPosZ;

    private float* _lhsRotX, _lhsRotY, _lhsRotZ, _lhsRotW;
    private float* _rhsRotX, _rhsRotY, _rhsRotZ, _rhsRotW;

    private float* _outPosX, _outPosY, _outPosZ;
    private float* _outRotX, _outRotY, _outRotZ, _outRotW;

    public NetworkInterpolationService(int initialCapacity = 128)
    {
        _isAvxSupported = Fma.IsSupported && Avx.IsSupported;
        if (_isAvxSupported)
        {
            AllocateAlignedMemory(initialCapacity);
        }
    }

    public void Register(INetworkTransformCentralized target) => _targets.Add(target);
    public void Unregister(INetworkTransformCentralized target) => _targets.Remove(target);

    public void ProcessBatch(float delta)
    {
        int total = _targets.Count;
        if (total == 0) return;

        // Fallback на исходный метод расширения для старых процессоров без AVX/FMA
        if (!_isAvxSupported)
        {
            for (int i = 0; i < total; i++)
                _targets[i].InterpolateTransform(delta);
            return;
        }

        if (total > _capacity)
        {
            AllocateAlignedMemory(Math.Max(total, _capacity * 2));
        }

        int activeCount = 0;
        const float teleportThresholdSq = TELEPORT_THRESHOLD * TELEPORT_THRESHOLD;

        // ШАГ 1: Подготовка данных и запись напрямую по указателям без проверок границ
        for (int i = 0; i < total; i++)
        {
            var target = _targets[i];
            ConfigureVirtualRenderTick(target, delta);

            var buffer = target.RingBuffer;
            if (buffer == null || buffer.Count < 2) continue;

            float renderTick = target.VirtualRenderTick;
            int idx = 0;
            while (idx < buffer.Count - 2 && buffer[idx + 1].Tick < renderTick)
                idx++;

            ref NetworkState lhs = ref buffer[idx];
            ref NetworkState rhs = ref buffer[idx + 1];

            // Проверка на телепорт через квадрат расстояния
            if (lhs.Position.DistanceSquaredTo(rhs.Position) > teleportThresholdSq)
            {
                target.GlobalTransform = new Transform3D(new Basis(rhs.Rotation), rhs.Position);
                buffer.RemoveOldest(buffer.Count - 1);
                continue;
            }

            // Очистка устаревших состояний в кольцевом буфере за O(1)
            int dropCount = 0;
            while (buffer.Count - dropCount > 2 && buffer[dropCount].Tick < lhs.Tick)
                dropCount++;
            if (dropCount > 0) buffer.RemoveOldest(dropCount);

            if (buffer.Count > MAX_BUFFER_STATES)
                buffer.RemoveOldest(buffer.Count - MAX_BUFFER_STATES);

            float denom = rhs.Tick - lhs.Tick;
            float tVal = denom > 0f ? Math.Clamp((renderTick - lhs.Tick) / denom, 0f, 1f) : 1f;

            _activeIndices[activeCount] = i;
            _t[activeCount] = tVal;

            _lhsPosX[activeCount] = lhs.Position.X;
            _lhsPosY[activeCount] = lhs.Position.Y;
            _lhsPosZ[activeCount] = lhs.Position.Z;
            _rhsPosX[activeCount] = rhs.Position.X;
            _rhsPosY[activeCount] = rhs.Position.Y;
            _rhsPosZ[activeCount] = rhs.Position.Z;

            _lhsRotX[activeCount] = lhs.Rotation.X;
            _lhsRotY[activeCount] = lhs.Rotation.Y;
            _lhsRotZ[activeCount] = lhs.Rotation.Z;
            _lhsRotW[activeCount] = lhs.Rotation.W;
            _rhsRotX[activeCount] = rhs.Rotation.X;
            _rhsRotY[activeCount] = rhs.Rotation.Y;
            _rhsRotZ[activeCount] = rhs.Rotation.Z;
            _rhsRotW[activeCount] = rhs.Rotation.W;

            activeCount++;
        }

        if (activeCount == 0) return;

        // ШАГ 2: Выровненный AVX2-расчёт по 8 объектов за итерацию
        ComputeAvxAligned(activeCount);

        // ШАГ 3: Применение результатов к нодам
        for (int k = 0; k < activeCount; k++)
        {
            var target = _targets[_activeIndices[k]];
            var pos = new Vector3(_outPosX[k], _outPosY[k], _outPosZ[k]);
            var rot = new Quaternion(_outRotX[k], _outRotY[k], _outRotZ[k], _outRotW[k]);
            target.GlobalTransform = new Transform3D(new Basis(rot), pos);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void ComputeAvxAligned(int count)
    {
        Vector256<float> signMask = Vector256.Create(-0.0f);

        for (int i = 0; i < count; i += 8)
        {
            // LoadAlignedVector256 работает быстрее обычного Load, так как адрес гарантированно кратен 32 байтам
            var vt = Avx.LoadAlignedVector256(_t + i);

            // --- LERP ПОЗИЦИИ ---
            var lpx = Avx.LoadAlignedVector256(_lhsPosX + i);
            var lpy = Avx.LoadAlignedVector256(_lhsPosY + i);
            var lpz = Avx.LoadAlignedVector256(_lhsPosZ + i);

            var rpx = Avx.LoadAlignedVector256(_rhsPosX + i);
            var rpy = Avx.LoadAlignedVector256(_rhsPosY + i);
            var rpz = Avx.LoadAlignedVector256(_rhsPosZ + i);

            Avx.StoreAligned(_outPosX + i, Fma.MultiplyAdd(Avx.Subtract(rpx, lpx), vt, lpx));
            Avx.StoreAligned(_outPosY + i, Fma.MultiplyAdd(Avx.Subtract(rpy, lpy), vt, lpy));
            Avx.StoreAligned(_outPosZ + i, Fma.MultiplyAdd(Avx.Subtract(rpz, lpz), vt, lpz));

            // --- NLERP КВАТЕРНИОНОВ ---
            var lrx = Avx.LoadAlignedVector256(_lhsRotX + i);
            var lry = Avx.LoadAlignedVector256(_lhsRotY + i);
            var lrz = Avx.LoadAlignedVector256(_lhsRotZ + i);
            var lrw = Avx.LoadAlignedVector256(_lhsRotW + i);

            var rrx = Avx.LoadAlignedVector256(_rhsRotX + i);
            var rry = Avx.LoadAlignedVector256(_rhsRotY + i);
            var rrz = Avx.LoadAlignedVector256(_rhsRotZ + i);
            var rrw = Avx.LoadAlignedVector256(_rhsRotW + i);

            // Скалярное произведение (Dot) для проверки кратчайшего пути
            var dot = Avx.Multiply(lrx, rrx);
            dot = Fma.MultiplyAdd(lry, rry, dot);
            dot = Fma.MultiplyAdd(lrz, rrz, dot);
            dot = Fma.MultiplyAdd(lrw, rrw, dot);

            // Безветвлевая инверсия знака, если dot < 0
            var negMask = Avx.And(dot, signMask);
            rrx = Avx.Xor(rrx, negMask);
            rry = Avx.Xor(rry, negMask);
            rrz = Avx.Xor(rrz, negMask);
            rrw = Avx.Xor(rrw, negMask);

            // Линейная интерполяция
            var rx = Fma.MultiplyAdd(Avx.Subtract(rrx, lrx), vt, lrx);
            var ry = Fma.MultiplyAdd(Avx.Subtract(rry, lry), vt, lry);
            var rz = Fma.MultiplyAdd(Avx.Subtract(rrz, lrz), vt, lrz);
            var rw = Fma.MultiplyAdd(Avx.Subtract(rrw, lrw), vt, lrw);

            // Нормализация через быстрый обратный корень (ReciprocalSqrt)
            var lenSq = Avx.Multiply(rx, rx);
            lenSq = Fma.MultiplyAdd(ry, ry, lenSq);
            lenSq = Fma.MultiplyAdd(rz, rz, lenSq);
            lenSq = Fma.MultiplyAdd(rw, rw, lenSq);

            var invLen = Avx.ReciprocalSqrt(lenSq);

            Avx.StoreAligned(_outRotX + i, Avx.Multiply(rx, invLen));
            Avx.StoreAligned(_outRotY + i, Avx.Multiply(ry, invLen));
            Avx.StoreAligned(_outRotZ + i, Avx.Multiply(rz, invLen));
            Avx.StoreAligned(_outRotW + i, Avx.Multiply(rw, invLen));
        }
    }

    private void AllocateAlignedMemory(int requestedCapacity)
    {
        if (_rawMemory != null)
        {
            NativeMemory.AlignedFree(_rawMemory);
            _rawMemory = null;
        }

        // Округляем вверх до кратности 8 (8 float * 4 байта = 32 байта — строго кратно Alignment)
        _capacity = (requestedCapacity + 7) & ~7;

        nuint intBlockBytes = (nuint)(_capacity * sizeof(int));
        nuint floatChannelBytes = (nuint)(_capacity * sizeof(float));
        nuint totalBytes = intBlockBytes + floatChannelBytes * FloatChannelsCount;

        _rawMemory = (byte*)NativeMemory.AlignedAlloc(totalBytes, Alignment);

        // Раздаём выровненные указатели внутри единого куска памяти
        byte* ptr = _rawMemory;
        _activeIndices = (int*)ptr; ptr += intBlockBytes;

        _t = (float*)ptr; ptr += floatChannelBytes;

        _lhsPosX = (float*)ptr; ptr += floatChannelBytes;
        _lhsPosY = (float*)ptr; ptr += floatChannelBytes;
        _lhsPosZ = (float*)ptr; ptr += floatChannelBytes;
        _rhsPosX = (float*)ptr; ptr += floatChannelBytes;
        _rhsPosY = (float*)ptr; ptr += floatChannelBytes;
        _rhsPosZ = (float*)ptr; ptr += floatChannelBytes;

        _lhsRotX = (float*)ptr; ptr += floatChannelBytes;
        _lhsRotY = (float*)ptr; ptr += floatChannelBytes;
        _lhsRotZ = (float*)ptr; ptr += floatChannelBytes;
        _lhsRotW = (float*)ptr; ptr += floatChannelBytes;

        _rhsRotX = (float*)ptr; ptr += floatChannelBytes;
        _rhsRotY = (float*)ptr; ptr += floatChannelBytes;
        _rhsRotZ = (float*)ptr; ptr += floatChannelBytes;
        _rhsRotW = (float*)ptr; ptr += floatChannelBytes;
        _outPosX = (float*)ptr; ptr += floatChannelBytes;
        _outPosY = (float*)ptr; ptr += floatChannelBytes;
        _outPosZ = (float*)ptr; ptr += floatChannelBytes;
        _outRotX = (float*)ptr; ptr += floatChannelBytes;
        _outRotY = (float*)ptr; ptr += floatChannelBytes;
        _outRotZ = (float*)ptr; ptr += floatChannelBytes;
        _outRotW = (float*)ptr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ConfigureVirtualRenderTick(INetworkTransform target, float delta)
    {
        if (target.VirtualRenderTick < 0 || target.StateBuffer.Count < 2) return;

        float newestTick = target.StateBuffer[^1].Tick;
        float targetRenderTick = newestTick - INTERPOLATION_DELAY_TICKS;
        float error = targetRenderTick - target.VirtualRenderTick;
        float speed = 1f;
        
        if (Mathf.Abs(error) > TIME_SYNC_DEADZONE)
        {
            float adjustedError = error - Mathf.Sign(error) * TIME_SYNC_DEADZONE;
            speed = Mathf.Clamp(1f + adjustedError * TIME_SYNC_GAIN, MIN_TIME_SCALE, MAX_TIME_SCALE);
        }

        float tickDelta = delta * TICK_RATE * speed;
        target.VirtualRenderTick = Mathf.Min(target.VirtualRenderTick + tickDelta, newestTick);
    }

    public void Dispose()
    {
        if (_rawMemory != null)
        {
            NativeMemory.AlignedFree(_rawMemory);
            _rawMemory = null;
        }
        GC.SuppressFinalize(this);
    }

    ~NetworkInterpolationService() => Dispose();
}