using Godot;

namespace ROMA2.Shared.Interfaces;

public interface INetworkTransform
{
    Vector3 NetworkPosition { get; set; }
    Vector3 NetworkRotation { get; set; }
    Transform3D GlobalTransform { get; set; }
}