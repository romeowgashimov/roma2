using Godot;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Server.Logic.Extensions;

public static class INetworkTransformExtensions
{
    public static void SynchronizeTransform(this INetworkTransform network)
    {
        network.NetworkPosition = network.Position;
        network.NetworkQuaternion = network.Quaternion;

        network.ServerTick = (uint)Engine.GetPhysicsFrames();
    } 
}