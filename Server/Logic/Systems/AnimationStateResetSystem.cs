using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Systems;

/// <summary>
/// Работает в самом начале кадра и сбрасывает состояние анимации всех сущностей в Idle.
/// </summary>
public class AnimationStateResetSystem : QuerySystem<AnimationStateComponent>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref AnimationStateComponent animState, Entity _) =>
        {
            animState.Current = AnimationState.Idle;
        });
    }
}