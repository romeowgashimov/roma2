using System;
using Godot;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public class ClientInputSyncFactory : InputSynchronizerFactory, IDisposable
{
    private NetworkBridge _bridge;

    public ClientInputSyncFactory(
        Node root, 
        InputSynchronizerRepository syncRepository,
        NetworkBridge bridge) : base(root, syncRepository)
    {
        _bridge = bridge;

        _bridge.InputSyncCreation += Spawn;
    }
    
    public void Dispose()
    {
        _bridge.InputSyncCreation -= Spawn;

        GC.SuppressFinalize(this);
    }
}