using Godot;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Network;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Projectiles;

public class ServerProjectileFactory(
    ProjectilesConfiguration projCong, 
    NetworkBridge bridge,
    Node parentRoot,
    ProjectileRepository repository) : ProjectileExtendedFactory(projCong, bridge, parentRoot, repository)
{
    public override Projectile Spawn(byte[] data)
    {
        var projectile = base.Spawn(data);

        _bridge.Rpc(nameof(_bridge.OnProjectileCreated), data);
            
        return projectile;
    }

    public override void Reset(int id, int localId, Projectile projectile)
    {
        _bridge.Rpc(nameof(_bridge.OnProjectileDeleted), localId);

        base.Reset(id, localId, projectile);
    }
}