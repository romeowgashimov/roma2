using Friflo.Engine.ECS;

namespace ROMA2.Shared.Data;

public struct AnimationStateComponent : IComponent
{
    public AnimationState Previous;
    public AnimationState Current;
}

public enum AnimationState : byte
{
    Idle,
    Walk,
    Run,
    Jump,
    Attack
}