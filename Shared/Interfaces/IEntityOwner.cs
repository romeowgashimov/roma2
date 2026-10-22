using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace ROMA2.Shared.Interfaces;

public interface IEntityOwner
{
    Entity Entity { get; }
    
    int GetCollisions(List<Entity> results); 
}