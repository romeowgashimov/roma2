using Friflo.Engine.ECS;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Data;

public struct ProjectileProperties : IComponent
{
    // Неизменимый идентификатор проджектайла
    public int GlobalId;
    // Динамически присваиваемый в ходе игры
    public int LocalId;
    public Projectile Reference;
    public float DestroyDelay;
    public float CurrentTime;
}

public struct DestroyProjectileTag : ITag { }

public struct RangedAttackId : IComponent
{
    public int Value;
}

public struct RangedAttackCooldown : IComponent
{
    public float Cooldown;
    public float CurrentTime;
}