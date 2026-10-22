using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Services;
using ROMA2.Client.Logic.Systems;
using ROMA2.Client.Logic.ViewModels;
using ROMA2.Client.Logic.Views;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionView _charSelectView;
    [Export] private MultiplayerSpawner _charSpawner;
    [Export] private Node _inputRoot;
    
    private NetworkBridge _bridge;
    private EntityStore _store;
    private SystemGroup _group;

    public void Init(NetworkBridge bridge, Client client, EntityStore store, SystemGroup group)
    {
        _group = group;
        _store = store;

        CharactersConfiguration charConfig = new(false);
        long myPeer = client.Multiplayer.GetUniqueId();
        PlayerRepository playerRepos = new() { MyPeer = myPeer };

        _store.CreateEntity(new UniqueEntity("MyPeer"), new PlayerPeer { Value = myPeer });

        _group.Add(new CameraPivotControllerSystem());

        _bridge = bridge;

        CharacterSelectionService charSelectService = new(_bridge, playerRepos);
        CharacterSelectionViewModel charSelectViewModel = new(charSelectService);
        _charSelectView.Init(charSelectViewModel);

        CharacterFactory charFactory = new(charConfig, _charSpawner, _store);
        ClientInputSyncFactory inputSyncFactory = new(_inputRoot, myPeer, false, _store);

        _bridge.InputSyncCreation += inputSyncFactory.Create;

        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long peer, int characterId)
        {
            if (myPeer != peer) return;
            
            _charSelectView.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }
}