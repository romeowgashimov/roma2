using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using static ROMA2.Shared.Interfaces.INetworkTransform;

namespace ROMA2.Client.Logic.Extensions;

public static class NetworkTransformExtensions
{
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

        // Линейная интерполяция строго внутри сегмента [0;1].
        // Экстраполяции нет: шутер не должен рисовать то, чего сервер еще не подтвердил.
        float denominator = rhs.Tick - lhs.Tick;
        float t = denominator > 0f ? (renderTick - lhs.Tick) / denominator : 1f;
        t = Mathf.Clamp(t, 0f, 1f);

        var rot = lhs.Rotation.Slerp(rhs.Rotation, t);
        Vector3 pos = lhs.Position.Lerp(rhs.Position, t);

        target.GlobalTransform = new Transform3D(new Basis(rot), pos);

        // Линейной интерполяции хватает двух состояний —
        // всё, что старше текущего сегмента, выбрасываем.
        while (buffer.Count > 2 && buffer[0].Tick < lhs.Tick)
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

        // Без экстраполяции рендер-тик не имеет права обгонять самый свежий серверный тик:
        // дальше просто застынем в последнем подтвержденном состоянии.
        target.VirtualRenderTick = Mathf.Min(target.VirtualRenderTick + tickDelta, newestTick);
    }
}