using Friflo.Engine.ECS;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Data;

public struct RangedAttackReference : IComponent
{
    public Projectile Value;
}

public struct RangedAttackCooldown : IComponent
{
    public float Cooldown;
    public float CurrentTime;
}