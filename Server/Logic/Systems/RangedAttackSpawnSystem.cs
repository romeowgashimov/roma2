using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Server.Logic.Projectiles;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Systems;

public class RangedAttackSpawnSystem : QuerySystem<RangedAttackId, RangedAttackCooldown, InputSynchronizerReference>
{
    private ServerProjectileFactory _factory;

    public RangedAttackSpawnSystem(ServerProjectileFactory spawner)
    {
        _factory = spawner;
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((
            ref RangedAttackId attackRef, 
            ref RangedAttackCooldown attackCd, 
            ref InputSynchronizerReference syncRef,
            Entity entity) =>
        {
            var sync = syncRef.Value;
            var projectile = attackRef.Value;
            attackCd.CurrentTime += (float)sync.GetProcessDeltaTime();

            if (sync.IsPressed && attackCd.CurrentTime >= attackCd.Cooldown)
            {
                Projectile proj = _factory.Create(attackRef.Value);
                CommandBuffer.RemoveTag<Disabled>(proj.Entity.Id);

                attackCd.CurrentTime = 0;
            }
            
        });
    }
}