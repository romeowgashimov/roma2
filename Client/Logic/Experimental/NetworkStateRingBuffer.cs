using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Experimental;

public sealed class NetworkStateRingBuffer
{
    private readonly NetworkState[] _items;
    private readonly int _capacity;
    private readonly int _mask;
    private int _head;
    private int _count;

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
    }

    public NetworkStateRingBuffer(int minCapacity)
    {
        if (minCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(minCapacity));
        
        // Округляем до ближайшей степени двойки (например, 10 -> 16) для работы через битовую маску
        _capacity = (int)BitOperations.RoundUpToPowerOf2((uint)minCapacity);
        _mask = _capacity - 1;
        _items = new NetworkState[_capacity];
    }

    // Доступ по ссылке без единой проверки границ массива и без ветвлений
    public ref NetworkState this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int actualIndex = (_head + index) & _mask;
            ref NetworkState first = ref MemoryMarshal.GetArrayDataReference(_items);
            return ref Unsafe.Add(ref first, actualIndex);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveOldest(int removeCount = 1)
    {
        if (removeCount <= 0) return;
        if (removeCount >= _count)
        {
            Clear();
            return;
        }

        _head = (_head + removeCount) & _mask;
        _count -= removeCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _head = 0;
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void InsertSorted(in NetworkState state)
    {
        if (float.IsNaN(state.Tick) || float.IsInfinity(state.Tick)) return;

        ref NetworkState first = ref MemoryMarshal.GetArrayDataReference(_items);

        // Ищем место вставки с конца
        int i = _count;
        while (i > 0 && Unsafe.Add(ref first, (_head + i - 1) & _mask).Tick > state.Tick)
            i--;

        // Дубликат тика
        if (i < _count && Unsafe.Add(ref first, (_head + i) & _mask).Tick == state.Tick)
            return;

        // Устаревший пакет (старше самого старого в буфере)
        if (i == 0 && _count > 0)
            return;

        // Вытесняем самый старый элемент при заполнении
        if (_count == _capacity)
        {
            _head = (_head + 1) & _mask;
            _count--;
            i--;
        }

        // Сдвигаем только хвост при нарушении порядка пакетов
        for (int j = _count; j > i; j--)
        {
            Unsafe.Add(ref first, (_head + j) & _mask) = Unsafe.Add(ref first, (_head + j - 1) & _mask);
        }

        Unsafe.Add(ref first, (_head + i) & _mask) = state;
        _count++;
    }
}