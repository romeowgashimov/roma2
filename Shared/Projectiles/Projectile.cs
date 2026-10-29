using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Projectiles;

public abstract partial class Projectile : RigidBody3D, IEntityOwner
{
    [Export] public MultiplayerSynchronizer Sync;
    [Export] public bool IsPooled;

    public int Id { get; init; }
    public Entity Entity;
    public Entity EntityOwner { get; set; }

    Entity IEntityOwner.Entity { get => Entity; }

    protected Projectile()
    {
        Id = ProjectilesConfiguration.GetProjectileId(this);
    }

    public void EnableEntity() => Entity.Enabled = true;

    public int GetCollisions(List<Entity> results)
    {
        return 0;
    }

    public abstract void Init();

    public override void _EnterTree()
    {
        Sync.SetMultiplayerAuthority(1);
    }

}