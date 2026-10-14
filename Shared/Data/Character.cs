using Friflo.Engine.ECS;
using Godot;

namespace ROMA2.Shared.Data;

public abstract partial class Character : CharacterBody3D
{
    [Export] public float CurrentHealth { get; set; }
    public int EntityId { get; set; }

    public abstract void Init(EntityStore store);
}