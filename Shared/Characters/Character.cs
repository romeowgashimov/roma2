using System.Collections.Generic;
using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Characters;

public abstract partial class Character : CharacterBody3D, IEntityOwner
{
    [Export] public Vector3 NetworkPosition;
    [Export] public Vector3 NetworkRotation;

    [Export] public AnimationState AnimationState;

    [Export] public float CurrentHealth { get; set; }
    
    public int EntityId { get; set; }
    public long OwnerPeer { get; set; }
    public Entity Entity { get; set; }

    public int GetCollisions(List<Entity> results)
    {
        int count = GetSlideCollisionCount();
        int added = 0;
        
        for (int i = 0; i < count; i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is IEntityOwner obstacle)
            {
                results.Add(obstacle.Entity);
                added++;
            }
        }
        return added;
    }

    public abstract void Init(EntityStore store);
}