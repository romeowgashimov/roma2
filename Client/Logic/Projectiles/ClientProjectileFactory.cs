using System;
using Godot;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Network;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Projectiles;

public class ClientProjectileFactory : ProjectileExtendedFactory, IDisposable
{
    public ClientProjectileFactory(
        ProjectilesConfiguration projCong, 
        NetworkBridge bridge,
        Node parentRoot,
        ProjectileRepository repository) : base(projCong, bridge, parentRoot, repository)
    {
        _bridge.ProjectileCreated += Spawn;
        _bridge.ProjectileDeleted += Reset;
    }

    public override Projectile Spawn(byte[] data)
    {
        var projectile = base.Spawn(data);

        projectile.Visible = true;
        
        return projectile;
    }

    public override void Reset(Projectile projectile)
    {
        base.Reset(projectile);

        projectile.Visible = false;
    }

    public void Reset(int reference)
    {
        var projectile = _repository.Get(reference);

        Reset(projectile);
    }

    public void Dispose()
    {
        _bridge.ProjectileCreated -= Spawn;
        _bridge.ProjectileDeleted -= Reset;

        GC.SuppressFinalize(this);
    }
}