using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientCharacter
{
    AnimationPlayer AnimationPlayer { get; }
    Character Character { get; }

    void InitClient(EntityStore store)
    {
        store
            .CreateEntity(
                new CharacterReference { Value = Character }, 
                new AnimationPlayerReference { Value = AnimationPlayer },
                new AnimationStateComponent { Previous = AnimationState.Idle })
            .AddTag<InterpolateTag>();
    }
}