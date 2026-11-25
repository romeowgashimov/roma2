using Godot;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Extensions;

public static class CharacterExtensions
{
    private const string IDLE_STATE = "Idle_anm"; 
    private const string WALK_STATE = "Run_anm";

    public static void Animate(this Character character, AnimationState state)
    {
        if (character is IClientCharacter clientCharacter)
            switch (state)
            {
                case AnimationState.Idle:
                    clientCharacter.AnimationStateMachine.Travel(IDLE_STATE);
                    character.PreviousAnimationState = character.AnimationState;
                    break;
                    
                case AnimationState.Walk:
                    clientCharacter.AnimationStateMachine.Travel(WALK_STATE);
                    character.PreviousAnimationState = character.AnimationState;
                    break;
            }
    }
}