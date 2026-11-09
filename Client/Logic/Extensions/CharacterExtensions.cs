using ROMA2.Client.Logic.Interfaces;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Extensions;

public static class CharacterExtensions
{
    public static void Animate(this Character character, AnimationState state)
    {
        if (character.PreviousAnimationState == character.AnimationState) return;

        if (character is IClientCharacter clientCharacter)
            switch (character.AnimationState)
            {
                case AnimationState.Idle:
                    clientCharacter.AnimationPlayer.Play("Idle_anm");
                    character.PreviousAnimationState = character.AnimationState;
                    break;
                    
                case AnimationState.Walk:
                    clientCharacter.AnimationPlayer.Play("Run_anm");
                    character.PreviousAnimationState = character.AnimationState;
                    break;
            }
    }
}