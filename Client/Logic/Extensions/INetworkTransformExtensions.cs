using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using static ROMA2.Shared.Interfaces.INetworkTransform;

namespace ROMA2.Client.Logic.Extensions;

public static class NetworkTransformExtensions
{
    private static float ResolveMaxSpeed(this INetworkTransform t) =>
        t.NetworkMaxSpeed > 0f ? t.NetworkMaxSpeed : MAX_SPEED_FALLBACK;

    public static void InterpolateTransform(this INetworkTransform target, float delta)
    {
        target.ConfigureVirtualRenderTick(delta);

        var buffer = target.StateBuffer;
        if (buffer == null || buffer.Count < 2) return;

        float renderTick = target.VirtualRenderTick;
        int idx = 0;

        while (idx < buffer.Count - 2 && buffer[idx + 1].Tick < renderTick)
            idx++;

        NetworkState lhs = buffer[idx];
        NetworkState rhs = buffer[idx + 1];

        float segmentLen = (rhs.Position - lhs.Position).Length();

        // Мгновенный перенос при телепортации или респавне
        if (segmentLen > TELEPORT_THRESHOLD)
        {
            target.GlobalTransform = new Transform3D(new Basis(rhs.Rotation), rhs.Position);
            while (buffer.Count > 1) buffer.RemoveAt(0);
            return;
        }

        float denominator = rhs.Tick - lhs.Tick;
        float t = denominator > 0f ? (renderTick - lhs.Tick) / denominator : 0f;

        bool isNewestSegment = idx == buffer.Count - 2;
        float maxT = isNewestSegment ? 1f + MAX_EXTRAPOLATION_SLACK : 1f;
        t = Mathf.Clamp(t, 0f, maxT);

        // Ограничение дистанции при экстраполяции
        if (isNewestSegment && t > 1f)
        {
            if (segmentLen > 0.0001f)
            {
                float extrapolationTime = MAX_EXTRAPOLATION_TICKS / TICK_RATE;
                float maxDist = target.ResolveMaxSpeed() * extrapolationTime;
                float tDist = 1f + maxDist / segmentLen;
                t = Mathf.Min(t, tDist);
            }
            else t = 1f;
        }

        // Поворот не экстраполируем дальше 1.0, чтобы избежать рывков угла
        var rot = lhs.Rotation.Slerp(rhs.Rotation, Mathf.Min(t, 1f));

        // Позиция: сплайн при наличии 4 точек и t <= 1, иначе линейная интерполяция/экстраполяция
        Vector3 pos;
        if (t <= 1f && idx >= 1 && idx + 2 < buffer.Count)
        {
            Vector3 p0 = buffer[idx - 1].Position;
            Vector3 p1 = lhs.Position;
            Vector3 p2 = rhs.Position;
            Vector3 p3 = buffer[idx + 2].Position;
            pos = CatmullRom(p0, p1, p2, p3, t);
        }
        else pos = lhs.Position.Lerp(rhs.Position, t);
        

        target.GlobalTransform = new Transform3D(new Basis(rot), pos);

        // Оставляем 1 прошлый тик (idx - 1) для работы сплайна Catmull-Rom
        int keepFromIdx = Mathf.Max(0, idx - 1);
        float oldestNeededTick = buffer[keepFromIdx].Tick;

        while (buffer.Count > 4 && buffer[0].Tick < oldestNeededTick)
            buffer.RemoveAt(0);

        while (buffer.Count > MAX_BUFFER_STATES)
            buffer.RemoveAt(0);
    }

    public static void OnNetworkTickReceived(this INetworkTransform network)
    {
        var buffer = network.StateBuffer;
        var serverTick = network.ServerTick;

        if (float.IsNaN(serverTick) || float.IsInfinity(serverTick)) return;

        int i = buffer.Count;
        while (i > 0 && buffer[i - 1].Tick > serverTick)
            i--;

        if (i < buffer.Count && buffer[i].Tick == serverTick) return;
        if (i == 0 && buffer.Count > 0) return;

        buffer.Insert(i, new NetworkState
        {
            Position = network.NetworkPosition,
            Rotation = network.NetworkQuaternion,
            Tick = serverTick
        });

        if (network.VirtualRenderTick < 0)
            network.VirtualRenderTick = serverTick - INTERPOLATION_DELAY_TICKS;
    }

    private static void ConfigureVirtualRenderTick(this INetworkTransform target, float delta)
    {
        if (target.VirtualRenderTick < 0 || target.StateBuffer.Count < 2) return;

        float newestTick = target.StateBuffer[^1].Tick;
        float targetRenderTick = newestTick - INTERPOLATION_DELAY_TICKS;
        float error = targetRenderTick - target.VirtualRenderTick;

        // Игнорируем мелкий сетевой джиттер внутри мертвой зоны
        float speed = 1f;
        if (Mathf.Abs(error) > TIME_SYNC_DEADZONE)
        {
            float adjustedError = error - Mathf.Sign(error) * TIME_SYNC_DEADZONE;
            speed = Mathf.Clamp(1f + adjustedError * TIME_SYNC_GAIN, MIN_TIME_SCALE, MAX_TIME_SCALE);
        }

        float tickDelta = delta * TICK_RATE * speed;
        float maxTick = newestTick + MAX_EXTRAPOLATION_TICKS;
        target.VirtualRenderTick = Mathf.Min(target.VirtualRenderTick + tickDelta, maxTick);
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }
}