using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Effects;
using ROMA2.Client.Logic.Extensions;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Client.Logic.Characters;

public partial class ClientAurora : Aurora, IClientPlayableCharacter, IClientEffectable
{
    [Export] public Camera3D Camera { get; set; }
    [Export] public CameraPivotController PivotController { get; set; }
    [Export] public AnimationTree AnimationTree { get; set; }
    public override ClientInput Input { get; set; }
    public AnimationNodeStateMachinePlayback AnimationStateMachine { get; set; }
    public EffectFactory EffectFactory { get; set; }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        AnimationStateChanged += this.Animate;
        AnimationStateChanged += AnimateAuroraRangedAttack;
        Synchronizer.Synchronized += this.OnNetworkTickReceived;
        SyncEffect += this.ClientSyncEffect;
    }

    protected override void OnTick(float delta)
    {
        this.InterpolateTransform(delta);
    }

    private async void AnimateAuroraRangedAttack(AnimationState state)
    {
        if (state == AnimationState.Attack)
        {
            PreviousAnimationState = state;

            if (HorizontalAttack) AnimationStateMachine.Travel("Attack1");
            else AnimationStateMachine.Travel("Crit");
        }

        if (state == AnimationState.Cast && CastOrUseAbilityIndex == 1)
        {
            PreviousAnimationState = state;
            
            AnimationStateMachine.Travel(CutCount.ToString());
        }

        // Здесь предыдущее состояние не храним, 
        // так как сами настраиваем переход на предыдущую анимацию
        if (state == AnimationState.Cast && CastOrUseAbilityIndex == 2)
        {
            AnimationStateMachine.Travel("Spell2");

            await Task.Delay(TimeSpan.FromSeconds(0.25));

            this.Animate(PreviousAnimationState);
        }
    }
}