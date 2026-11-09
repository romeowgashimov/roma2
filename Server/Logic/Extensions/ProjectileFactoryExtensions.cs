using MemoryPack;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Extensions;

public static class ProjectileFactoryExtensions
{
    public static Projectile Create(this ProjectileExtendedFactory factory, int configId)
    {
        var data = new ProjectileCreationData
        {
            GlobalId = configId,
            LocalId = factory.ProjConfig.ProjectilesCount,
            OwnerPeer = long.MinValue
        };

        var sendData = MemoryPackSerializer.Serialize(data);
        
        return factory.Spawn(sendData);
    }
}