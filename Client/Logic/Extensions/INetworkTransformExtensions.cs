using Godot;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Client.Logic.Extensions;

public static class INetworkTransformExtensions
{
    private const float INTERPOLATION_COEFFICIENT = 10f;
    
    public static void InterpolateTransform(this INetworkTransform target, float deltaTime)
    {
        if (target == null) return;

        float dt = deltaTime;
        float t = INTERPOLATION_COEFFICIENT * dt;

        Transform3D currentTransform = target.GlobalTransform;

        Transform3D targetTransform = currentTransform;
        targetTransform.Origin = target.NetworkPosition;
        targetTransform.Basis = new Basis(Quaternion.FromEuler(target.NetworkRotation));

        target.GlobalTransform = currentTransform.InterpolateWith(targetTransform, t);
    }
}