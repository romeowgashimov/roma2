using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Characters;

public partial class ServerAurora : Aurora
{
    [Export] private Projectile _rangedAttack;
    [Export] private float _rangedAttackCooldown;

    public override void Init(EntityStore store)
    {
        Entity.AddComponent(new RangedAttackReference { Value = _rangedAttack });
        Entity.AddComponent(new RangedAttackCooldown { Cooldown = _rangedAttackCooldown });


    }
}