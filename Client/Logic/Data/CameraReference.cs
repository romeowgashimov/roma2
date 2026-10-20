using Friflo.Engine.ECS;
using Godot;

namespace ROMA2.Client.Logic.Data;

public struct CameraReference : IComponent
{
    public Camera3D Value;
}