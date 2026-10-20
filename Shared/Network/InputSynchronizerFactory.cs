using System;
using Godot;

namespace ROMA2.Shared.Network;

public class InputSynchronizerFactory
{
    private string _path = "res://Shared/Network/InputSynchronizer.tscn";
    private Node _root;
    private long _myPeer;
    private bool _isServer;

    public event Action<InputSynchronizer> InputSyncCreated;

    public InputSynchronizerFactory(Node root, long myPeer, bool isServer)
    {
        _root = root;
        _myPeer = myPeer;
        _isServer = isServer;
    }

    public InputSynchronizer Create(long data)
    {
        return Spawn(data);
    }

    public InputSynchronizer Create()
    {
        return Spawn(default);
    }

    private InputSynchronizer Spawn(long peer)
    {
        var inputSync = GD.Load<PackedScene>(_path).Instantiate<InputSynchronizer>();

        long ownerPeer = _isServer ? peer : _myPeer;
            
        inputSync.Name = $"InputSynchronizer_{ownerPeer}";

        inputSync.SetMultiplayerAuthority((int)ownerPeer);

        _root.AddChild(inputSync);

        var sync = inputSync.GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");

        sync.PublicVisibility = false;
        sync.SetVisibilityFor(1, true);

        InputSyncCreated?.Invoke(inputSync);

        return inputSync;
    }
}