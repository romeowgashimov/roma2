using Friflo.Engine.ECS;
using Godot;

namespace ROMA2.Shared.Data;

public abstract partial class Character : CharacterBody3D
{
    [Export] public Vector3 NetworkPosition;
    [Export] public Vector3 NetworkRotation;

    [Export] public AnimationState AnimationState;

    [Export] public float CurrentHealth { get; set; }
    
    public int EntityId { get; set; }
    public long OwnerPeer { get; set; }

    public abstract void Init(EntityStore store);
}