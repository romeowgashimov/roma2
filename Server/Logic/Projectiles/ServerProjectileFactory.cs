using Friflo.Engine.ECS;
using Godot;
using MemoryPack;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Projectiles;

public class ServerProjectileFactory(
    ProjectilesConfiguration projCong, 
    EntityStore store, 
    NetworkBridge bridge,
    Node parentRoot,
    ProjectileRepository repository) : ProjectileExtendedFactory(projCong, store, bridge, parentRoot, repository)
{
    public Projectile Create(int id)
    {
        var data = new ProjectileCreationData
        {
            GlobalId = id,
            LocalId = _projConfig.ProjectilesCount,
            OwnerPeer = long.MinValue
        };

        var sendData = MemoryPackSerializer.Serialize(data);
        
        return Spawn(sendData);
    }

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