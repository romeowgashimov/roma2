using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;

namespace ROMA2.Client.Logic.Core;

public class ClientInputSyncFactory(Node root, long myPeer, bool isServer, EntityStore store) 
    : InputSynchronizerFactory(root, myPeer, isServer)
{
    protected override InputSynchronizer Spawn(long peer)
    {
        var sync = base.Spawn(peer);

        store.CreateEntity(new InputSynchronizerReference { Value = sync });

        return sync;
    }
}