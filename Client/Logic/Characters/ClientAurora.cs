using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Extensions;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Client.Logic.Characters;

public partial class ClientAurora : Aurora, IClientPlayableCharacter
{
    [Export] public Camera3D Camera { get; set; }
    [Export] public CameraPivotController PivotController { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }
    public override ClientInput Input { get; set; }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        AnimationStateChanged += this.Animate;
        AnimationStateChanged += AnimateAuroraRangedAttack;
        Synchronizer.Synchronized += this.OnNetworkTickReceived;
    }

    public override void Tick(float delta)
    {
        this.InterpolateTransform(delta);
    }

    private void AnimateAuroraRangedAttack(AnimationState state)
    {
        if (state == AnimationState.Attack)
        {
            PreviousAnimationState = state;

            if (HorizontalAttack) AnimationPlayer.Play("Attack1");
            else AnimationPlayer.Play("Crit");
        }
    }
}