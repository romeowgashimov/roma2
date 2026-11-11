using Godot;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Server.Logic.Extensions;

public static class INetworkTransformExtensions
{
    public static void SynchronizeTransform(this INetworkTransform network)
    {
        network.NetworkPosition = network.Position;
        network.NetworkQuaternion = network.Quaternion;

        if (!Mathf.IsEqualApprox(network.NetworkMaxSpeed, network.ServerMaxSpeed))
            network.NetworkMaxSpeed = network.ServerMaxSpeed;

        network.ServerTick = (uint)Engine.GetPhysicsFrames();
    } 
}