using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Core;

/// <summary>
/// Работает в самом конце и отправляет состояние анимации на клиент
/// </summary>
public class AnimationStateRegistrationSystem : QuerySystem<AnimationStateComponent, CharacterReference>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AnimationStateComponent animState, ref CharacterReference charRef, Entity entity) =>
        {
            charRef.Value.AnimationState = animState.Current; 
        });
    }
}