using Friflo.Engine.ECS;

namespace ROMA2.Shared.Data;

public struct MaxHealth : IComponent
{
    public int Value;
}

public struct CurrentHealth : IComponent
{
    public float Value;
}

public struct MoveSpeed : IComponent
{
    public int Value;
}