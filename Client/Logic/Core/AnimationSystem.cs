using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public class AnimationSystem : QuerySystem<CharacterReference, AnimationPlayerReference, AnimationStateComponent>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((
            ref CharacterReference charRef, 
            ref AnimationPlayerReference animRef, 
            ref AnimationStateComponent animComp, 
            Entity entity) =>
        {
            AnimationState animState = charRef.Value.AnimationState;
            animComp.Current = animState;

            if (animComp.Previous == animComp.Current) return;

            AnimationPlayer animPlayer = animRef.Value;

            switch (animState)
            {
                case AnimationState.Idle:
                    animPlayer.Play("Idle_anm");
                    animComp.Previous = animComp.Current;
                    break;
                case AnimationState.Walk:
                    animPlayer.Play("Run_anm");
                    animComp.Previous = animComp.Current;
                    break;
            }
        });
    }
}