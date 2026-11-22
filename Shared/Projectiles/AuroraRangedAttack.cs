using Godot;

namespace ROMA2.Shared.Projectiles;

public abstract partial class AuroraRangedAttack : Projectile
{
    public Vector3 Direction { get; set; }
}