using System.Collections.Generic;
using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Projectiles;

public abstract partial class Projectile : RigidBody3D, IEntityOwner
{
    public Entity Entity { get; set; }
    public Entity EntityOwner { get; set; }

    public int GetCollisions(List<Entity> results)
    {
        return 0;
    }

    public abstract void Init();
}