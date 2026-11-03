using Friflo.Engine.ECS;

namespace ROMA2.Shared.Data;

public struct DestroyProjectileTag : ITag { }

// Неизменяемый идентификатор проджектайла
public struct RangedAttackGlobalId : IComponent
{
    public int Value;
}

// Динамически присваиваемый идентификатор в ходе игры
public struct ProjectileReference : IComponent
{
    public int Value;
}

public struct RangedAttackCooldown : IComponent
{
    public float Cooldown;
    public float CurrentTime;
}