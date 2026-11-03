using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Server.Logic.Projectiles;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Systems;

public class RangedAttackSpawnSystem : QuerySystem<RangedAttackGlobalId, RangedAttackCooldown, InputSynchronizerReference>
{
    private ServerProjectileFactory _factory;
    private InputSynchronizerRepository _syncs;

    public RangedAttackSpawnSystem(ServerProjectileFactory spawner, InputSynchronizerRepository syncs)
    {
        _factory = spawner;
        _syncs = syncs;
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((
            ref RangedAttackGlobalId attackRef, 
            ref RangedAttackCooldown attackCd, 
            ref InputSynchronizerReference syncRef,
            Entity entity) =>
        {
            var sync = _syncs.Get(syncRef.OwnerPeer);

            var projectile = attackRef.ProjectileId;
            attackCd.CurrentTime += (float)sync.GetProcessDeltaTime();

            if (sync.IsPressed && attackCd.CurrentTime >= attackCd.Cooldown)
            {
                Projectile proj = _factory.Create(attackRef.ProjectileId);
                CommandBuffer.RemoveTag<Disabled>(proj.Entity.Id);

                attackCd.CurrentTime = 0;
            }
            
        });
    }
}