using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace ROMA2.Shared.Data;

public struct CollisionsReference : IComponent
{
    public List<Entity> Entities;
}