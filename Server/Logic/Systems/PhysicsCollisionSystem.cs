using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Systems;

public class PhysicsCollisionSystem : QuerySystem<EntityOwnerReference, CollisionsReference>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity(static (ref EntityOwnerReference ownerRef, ref CollisionsReference collisionsRef, Entity entity) =>
        {
            collisionsRef.Entities ??= [];
            collisionsRef.Entities.Clear();

            ownerRef.Value.GetCollisions(collisionsRef.Entities);
        });
    }
}

