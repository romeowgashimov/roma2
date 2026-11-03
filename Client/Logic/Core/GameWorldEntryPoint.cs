using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Characters;
using ROMA2.Client.Logic.Projectiles;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Client.Logic.Services;
using ROMA2.Client.Logic.Systems;
using ROMA2.Client.Logic.ViewModels;
using ROMA2.Client.Logic.Views;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;
using ROMA2.Shared.Systems;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionView _charSelectView;
    [Export] private MultiplayerSpawner _charSpawner;
    [Export] private Node _inputRoot;
    [Export] private Node _projectileRoot;
    
    private NetworkBridge _bridge;
    private EntityStore _store;
    private SystemGroup _group;
    private PlayerRepository _playerRepository;

    public void Init(
        NetworkBridge bridge,
        Client client,
        EntityStore store,
        SystemGroup group,
        PlayerRepository playerRepository, 
        InputHandler inputHandler)
    {
        _group = group;
        _store = store;
        _playerRepository = playerRepository;

        CharactersConfiguration charConfig = new(false);
        long myPeer = client.Multiplayer.GetUniqueId();
        PlayerRepository playerRepos = new() { MyPeer = myPeer };

        _store.CreateEntity(new UniqueEntity("MyPeer"), new PlayerPeer { Value = myPeer });

        CameraPivotControllerRepositoty pivotRepository = new();
        InputSynchronizerRepository syncRepository = new();

        EndSimulationCommandBufferSystem endSimulationSystem = new();
        _group.Add(new LocalPivotControllerSystem(pivotRepository));
        _group.Add(new ServerPivotControllerSystem(pivotRepository, syncRepository));
        _group.Add(new InputSynchronizationSystem(syncRepository, pivotRepository));
        _group.Add(endSimulationSystem);

        _bridge = bridge;

        CharacterSelectionService charSelectService = new(_bridge, playerRepos);
        CharacterSelectionViewModel charSelectViewModel = new(charSelectService);
        _charSelectView.Init(charSelectViewModel);

        ClientPlayableCharFactory charFactory = new(charConfig, _charSpawner, _store, pivotRepository);

        ClientInputSyncFactory inputSyncFactory = new(_inputRoot, syncRepository, _store, myPeer);
        _bridge.InputSyncCreation += inputSyncFactory.Create;
        
        ProjectilesConfiguration projConfig = new(false);
        ProjectileRepository rep = new();
        ClientProjectileFactory projFactory = new(projConfig, store, bridge, _projectileRoot, rep);

        AllyViewSharingService viewSharingService = new(
            _bridge, pivotRepository, myPeer, _store, endSimulationSystem.EndSimulationCommandBuffer, _playerRepository, inputHandler, syncRepository);

        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long peer, int characterId)
        {
            if (myPeer != peer) return;
            
            _charSelectView.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }
}