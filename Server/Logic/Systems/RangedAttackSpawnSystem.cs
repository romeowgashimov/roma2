using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Systems;

public class RangedAttackSpawnSystem : QuerySystem<RangedAttackReference, RangedAttackCooldown>
{
    private MultiplayerSpawner _projectileSpawner;

    public RangedAttackSpawnSystem(MultiplayerSpawner spawner)
    {
        _projectileSpawner = spawner;
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity(static (
            ref RangedAttackReference attackRef, 
            ref RangedAttackCooldown attackCd, 
            Entity entity) =>
        {
            
        });
    }
}