using Godot;
using ROMA2.Client.Logic.Characters;
using ROMA2.Client.Logic.Projectiles;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Client.Logic.Services;
using ROMA2.Client.Logic.ViewModels;
using ROMA2.Client.Logic.Views;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionView _charSelectView;
    [Export] private MultiplayerSpawner _charSpawner;
    [Export] private Node _inputRoot;
    [Export] private Node _projectileRoot;
    
    private NetworkBridge _bridge;
    private PlayerRepository _playerRepository;
    private CharacterRepository _charRepo;
    private ProjectileRepository _projRepo;

    private AllyViewSharingService _viewSharingService;
    private InputSynchronizerService _syncService;
    private CameraPivotControllerService _pivotService;

    public void Init(
        NetworkBridge bridge,
        Client client,
        PlayerRepository playerRepository, 
        InputHandler inputHandler)
    {
        _bridge = bridge;
        _playerRepository = playerRepository;

        long myPeer = client.Multiplayer.GetUniqueId();
        GlobalConfiguration.MyPeer = myPeer;

        PlayableCharacterConfiguration charConfig = new(false);
        ProjectilesConfiguration projConfig = new(false);

        CameraPivotControllerRepository pivotRepository = new();
        InputSynchronizerRepository syncRepository = new();
        _projRepo = new();
        _charRepo = new();

        ClientInputSyncFactory inputSyncFactory = new(_inputRoot, syncRepository, _bridge);
        ClientProjectileFactory projFactory = new(
            projConfig, bridge, _projectileRoot, _projRepo);
        ClientPlayableCharFactory charFactory = new(
            charConfig, _charSpawner, _charRepo, pivotRepository, inputHandler, projFactory);

        CharacterSelectionService charSelectService = new(_bridge, _playerRepository);
        _syncService = new(syncRepository, pivotRepository, myPeer);
        _pivotService = new(pivotRepository);
        _viewSharingService = new(
            _bridge, pivotRepository, myPeer, _playerRepository, inputHandler, syncRepository, _pivotService);

        CharacterSelectionViewModel charSelectViewModel = new(charSelectService);
        _charSelectView.Init(charSelectViewModel);
        
        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long peer, int characterId)
        {
            if (myPeer != peer) return;
            
            _charSelectView.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        
        _charRepo.Tick(dt);
        _projRepo.Tick(dt);

        _pivotService.Tick(dt);
        _viewSharingService.Tick(dt);
        _syncService.Tick(dt);
    }
}