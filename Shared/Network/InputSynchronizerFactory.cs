using Godot;
using ROMA2.Shared.Repositories;

namespace ROMA2.Shared.Network;

public abstract class InputSynchronizerFactory
{
    private string _path = "res://Shared/Network/InputSynchronizer.tscn";
    private Node _root;
    private InputSynchronizerRepository _syncRepository;

    public InputSynchronizerFactory(Node root, InputSynchronizerRepository syncRepository)
    {
        _root = root;
        _syncRepository = syncRepository;
    }

    // В качестве ссылки на объект reference передаём пир владельца только в случае, 
    // когда объект создаётся один раз и навсегда. Инпут синхронизатор таким является.
    public virtual InputSynchronizer Spawn(long ownerPeer)
    {
        var inputSync = GD.Load<PackedScene>(_path).Instantiate<InputSynchronizer>();
           
        inputSync.Name = $"InputSynchronizer_{ownerPeer}";
        inputSync.Reference = ownerPeer;

        inputSync.SetMultiplayerAuthority((int)ownerPeer);

        _root.AddChild(inputSync);

        var sync = inputSync.Synchronizer;

        sync.PublicVisibility = false;
        sync.SetVisibilityFor(1, true);

        _syncRepository.Add(ownerPeer, inputSync);

        return inputSync;
    }
}