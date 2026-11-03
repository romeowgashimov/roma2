using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public class ClientInputSyncFactory(
    Node root, 
    InputSynchronizerRepository syncRepository, 
    EntityStore store, 
    long myPeer)
    : InputSynchronizerFactory(root, syncRepository)
{
    protected override InputSynchronizer Spawn(long peer)
    {
        var sync = base.Spawn(peer);

        if (myPeer == peer)
        {
            var entity = store.CreateEntity(new InputSynchronizerReference { OwnerPeer = peer });
            entity.AddTag<LocalPlayerTag>();
        }

        return sync;
    }
}