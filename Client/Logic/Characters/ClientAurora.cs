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
    [Export] public AnimationTree AnimationTree { get; set; }
    public override ClientInput Input { get; set; }
    public AnimationNodeStateMachinePlayback AnimationStateMachine { get; set; }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        AnimationStateChanged += this.Animate;
        AnimationStateChanged += AnimateAuroraRangedAttack;
        Synchronizer.Synchronized += this.OnNetworkTickReceived;
    }

    protected override void OnTick(float delta)
    {
        this.InterpolateTransform(delta);
    }

    protected override void Razrezi()
    {
        PreviousAnimationState = AnimationState.Cast;

        AnimationStateMachine.Travel(CutCount.ToString());
    }

    private void AnimateAuroraRangedAttack(AnimationState state)
    {
        if (state == AnimationState.Attack)
        {
            PreviousAnimationState = state;

            if (HorizontalAttack) AnimationStateMachine.Travel("Attack1");
            else AnimationStateMachine.Travel("Crit");
        }
    }
}