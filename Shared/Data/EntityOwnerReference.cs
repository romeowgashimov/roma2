using Friflo.Engine.ECS;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Data;

public struct EntityOwnerReference : IComponent
{
    public IEntityOwner Value;
}