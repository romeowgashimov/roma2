using Friflo.Engine.ECS;
using ROMA2.Shared.Characters;

namespace ROMA2.Shared.Data;

public struct CharacterReference : IComponent
{
    public Character Value;
}