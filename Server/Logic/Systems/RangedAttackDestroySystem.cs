using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Systems;

public class RangedAttackDestroySystem : QuerySystem<ProjectileProperties>
{
    private ProjectileExtendedFactory _factory;

    public RangedAttackDestroySystem(ProjectileExtendedFactory factory)
    {
        _factory = factory;

        Filter.AllTags(Tags.Get<DestroyProjectileTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref ProjectileProperties properties, Entity entity) =>
        {
            properties.CurrentTime = 0;
            CommandBuffer.RemoveTag<DestroyProjectileTag>(entity.Id);
            CommandBuffer.AddTag<Disabled>(entity.Id);

            _factory.Reset(properties.GlobalId, properties.LocalId, properties.Reference);
        });
    }
}