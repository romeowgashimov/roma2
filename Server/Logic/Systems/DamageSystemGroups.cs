using Friflo.Engine.ECS.Systems;

namespace ROMA2.Server.Logic.Systems;

public class OutgoingDamageSystemGroup(string name) : SystemGroup(name);

public class IncomingDamageSystemGroup(string name) : SystemGroup(name);