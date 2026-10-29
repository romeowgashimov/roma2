using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Characters;

public partial class ServerAurora : Aurora
{
    [Export] private float _rangedAttackCooldown;

    public override void Init(EntityStore store)
    {
        Entity = store.CreateEntity();
        Entity.AddComponent(new RangedAttackId { Value = ProjectilesConfiguration.GetProjectileId(typeof(AuroraRangedAttack)) });
        GD.Print(ProjectilesConfiguration.GetProjectileId(typeof(AuroraRangedAttack)));
        Entity.AddComponent(new RangedAttackCooldown { Cooldown = _rangedAttackCooldown, CurrentTime = _rangedAttackCooldown });


    }
}