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

        float denominator = rhs.Tick - lhs.Tick;
        float t = denominator > 0f ? (renderTick - lhs.Tick) / denominator : 0f;

        bool isNewestSegment = idx == buffer.Count - 2;
        float maxT = isNewestSegment ? 1f + MAX_EXTRAPOLATION_SLACK : 1f;
        t = Mathf.Clamp(t, 0f, maxT);

        if (isNewestSegment && t > 1f)
        {
            float segmentLen = (rhs.Position - lhs.Position).Length();
            if (segmentLen > 0.0001f)
            {
                float extrapolationTime = MAX_EXTRAPOLATION_TICKS / TICK_RATE;
                float maxDist = target.ResolveMaxSpeed() * extrapolationTime;
                float tDist = 1f + maxDist / segmentLen;
                t = Mathf.Min(t, tDist);
            }
            else t = 1f;
        }

        var rot = lhs.Rotation.Slerp(rhs.Rotation, t);
        var pos = lhs.Position.Lerp(rhs.Position, t);

        target.GlobalTransform = new Transform3D(new Basis(rot), pos);

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
        while (i > 0 && buffer[i - 1].Tick > serverTick) i--;

        if (i < buffer.Count && buffer[i].Tick == serverTick) return;

        if (i == 0 && buffer.Count > 0) return;

        buffer.Insert(i, new NetworkState {
            Position = network.NetworkPosition,
            Rotation = network.NetworkQuaternion,
            Tick = serverTick});

        if (network.VirtualRenderTick < 0)
            network.VirtualRenderTick = serverTick - INTERPOLATION_DELAY_TICKS;
    }

    private static void ConfigureVirtualRenderTick(this INetworkTransform target, float delta)
    {
        if (target.VirtualRenderTick < 0 || target.StateBuffer.Count < 2) return;

        float newestTick = target.StateBuffer[^1].Tick;
        float targetRenderTick = newestTick - INTERPOLATION_DELAY_TICKS;
        float error = targetRenderTick - target.VirtualRenderTick;

        float speed = Mathf.Clamp(1f + error * TIME_SYNC_GAIN, 0.9f, 1.1f);

        float tickDelta = delta * TICK_RATE * speed;

        float maxTick = newestTick + MAX_EXTRAPOLATION_TICKS;
        target.VirtualRenderTick = Mathf.Min(target.VirtualRenderTick + tickDelta, maxTick);
    }
}