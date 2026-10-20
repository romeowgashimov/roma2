using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Characters;

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