using Godot;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Characters;

public abstract partial class Aurora : Character
{
    [Export] public int CutCount = 4;
    public int MaxCutCount = 4;
    [Export] public bool HorizontalAttack;
    // 0.05 для погрешности, иногда получалось 5, иногда 6, а нужно 6
    public float RazreziEndTime => 1.25f;
    protected float _razreziAnimationEndCounter = 0;
    protected const float RAZREZI_PROGRESSION_TIME = 0.2f; 
    // Мб, потом для синхронизации, чтобы точь-в-точь урон регался под анимации,
    // а не такой рассинхрон как у меня сейчас
    //protected float _razreziProgressionСumulativeTime;
    protected float _razreziProgressionTime = 0;
    protected const int DEFAULT_CUT_COUNT = 2;
    protected int _cutCountProgression = 0; 

    [Export] public Vector3[] PositionOffsets = new Vector3[2];
    [Export] public Vector3[] RotationOffsets = new Vector3[2];

    public void WriteRangedAttackOffset(Projectile projectile)
    {
        if (projectile is AuroraRangedAttack auroraProj)
        {
            auroraProj.Direction = Input.MouseDirection.Normalized();

            Vector3 targetPoint = auroraProj.GlobalPosition + auroraProj.Direction;
            auroraProj.LookAt(targetPoint, Vector3.Up);

            int index = HorizontalAttack ? 0 : 1;
            float angleInRadians = Mathf.DegToRad(RotationOffsets[index].Z);
            projectile.Position = Position + PositionOffsets[index];

            auroraProj.RotateObjectLocal(Vector3.Forward, angleInRadians);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void SetHorizontalAttack(bool isHorizontal)
    {
        HorizontalAttack = isHorizontal;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void SetCutCount(byte cutCount)
    {
        if (cutCount > 4) cutCount = 4;
        GD.Print(cutCount);

        CutCount = cutCount;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    protected abstract void Razrezi();

    protected void UpdateRazreziAnimationCooldown(float delta)
    {
        if (CastOrUseAbilityIndex == 1) 
        {
            if (_razreziAnimationEndCounter < RazreziEndTime)
                _razreziAnimationEndCounter += delta;
            else
            {
                CastOrUseAbilityIndex = byte.MinValue;
                _razreziAnimationEndCounter = 0;

                if (Velocity.LengthSquared() >= 0.25f)
                    AnimationState = AnimationState.Walk;
                else
                    AnimationState = AnimationState.Idle;
            }
        }
    }
}