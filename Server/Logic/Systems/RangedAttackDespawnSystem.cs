using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Systems;

public class RangedAttackDespawnSystem : QuerySystem<ProjectileProperties>
{
    public RangedAttackDespawnSystem()
    {
        Filter.WithoutAnyTags(Tags.Get<DestroyProjectileTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ProjectileProperties properties, Entity entity) =>
        {
            GD.Print(properties.CurrentTime);
            if (properties.DestroyDelay <= properties.CurrentTime)
            {
                CommandBuffer.AddTag<DestroyProjectileTag>(entity.Id);
            }

            properties.CurrentTime += (float)properties.Reference.GetProcessDeltaTime();
        });
    }
}