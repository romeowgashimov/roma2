using Godot;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Core;

public class ServerInputSyncFactory(
    Node root, 
    InputSynchronizerRepository syncRepository,
    NetworkBridge bridge) : InputSynchronizerFactory(root, syncRepository)
{
    public InputSynchronizer Create(long ownerPeer)
    {
        var input = Spawn((int)ownerPeer);

        bridge.Rpc(nameof(bridge.OnInputSynchronizerCreation), ownerPeer);
        
        return input;
    }
}