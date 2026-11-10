using Godot;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Characters;

public abstract partial class Aurora : Character
{
    [Export] public int CutCount;
    public int MaxCutCount = 4;

    [Export] public bool HorizontalAttack;
    [Export] public Vector3 PositionOffset;
    [Export] public Vector3[] RotationOffsets = new Vector3[2];

    public void WriteRangedAttackOffset(Projectile projectile)
    {
        int index = HorizontalAttack ? 0 : 1;
        projectile.Position = Position + PositionOffset;
        projectile.Rotation = RotationOffsets[index];
    }
}