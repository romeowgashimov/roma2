using Godot;
using ROMA2.Server.Logic.Characters;
using ROMA2.Server.Logic.Projectiles;
using ROMA2.Server.Logic.Repositories;
using ROMA2.Server.Logic.Services;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;
using static Godot.GD;
using static ROMA2.Server.Logic.Core.GameState;

namespace ROMA2.Server.Logic.Core;

public partial class EntryPoint : Node
{
    [ExportGroup("Network")]
    [Export] private NetworkBridge _networkBridge;
    [Export] private Server _server;
    [Export] private GameState _state = WaitPlayers;

    [ExportGroup("Character Factory")]
    [Export] private MultiplayerSpawner _charSpawner;
    [Export] private Node _inputRoot;

    [ExportGroup("Projectile Factory")]
    [Export] private Node _parentRoot;

    [ExportGroup("Spawn Configuration", "pref")]
    [ExportSubgroup("First Team")]
    [Export] private Vector3 _fFirstPosition;
    [Export] private Vector3 _fSecondPosition;
    [Export] private Vector3 _fThirdPosition;
    [Export] private Vector3 _fFourthPosition;
    [Export] private Vector3 _fFifthPosition;

    [ExportSubgroup("Second Team")]
    [Export] private Vector3 _sFirstPosition;
    [Export] private Vector3 _sSecondPosition;
    [Export] private Vector3 _sThirdPosition;
    [Export] private Vector3 _sFourthPosition;
    [Export] private Vector3 _sFifthPosition;

    private CharacterSelectionService _charSelectService;
    private PreGameInitializationService _initService;

    private ProjectileRepository _projRepo = new();
    private CharacterRepository _charRepo = new(); 
    private PlayerControllerRepository _contRepo = new();

    public override void _Ready()
    {
        long myPeer = _server.Multiplayer.GetUniqueId();

        SpawnConfiguration spawnConfig = new(
            [_fFirstPosition, _fSecondPosition, _fThirdPosition, _fFourthPosition, _fFifthPosition], 
            [_sFirstPosition, _sSecondPosition, _sThirdPosition, _sFourthPosition, _sFifthPosition]);
        PlayableCharacterConfiguration config = new(isServer: true);
        ProjectilesConfiguration projConfig = new(isServer: true);
        
        PlayerRepository playerRepos = new() { MyPeer = myPeer };

        InputSynchronizerRepository inputRepository = new();

        ServerProjectileFactory projFactory = new(projConfig, _networkBridge, _parentRoot, _projRepo);
        ServerPlayableCharFactory charFactory = new(config, _charSpawner, _charRepo, projFactory);
        ServerInputSyncFactory syncFactory = new(_inputRoot, inputRepository, _networkBridge);

        AllyViewSharingService viewSharingService = new(_networkBridge, playerRepos, inputRepository);
        _charSelectService = new(playerRepos, _networkBridge);
        _charSelectService.ChangeState += OnChangingState;
        _initService = new(playerRepos, spawnConfig, charFactory, syncFactory, _contRepo);
        _initService.ChangeState += OnChangingState;

        _server.Init(playerRepos);
        _server.ChangeState += OnChangingState;

        _server.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        switch (_state)
        {
            case WaitPlayers:
                break;
            case WaitChoices:
                _charSelectService.Run(dt);
                break;
            case Initialization:
                _initService.Run();
                break;
            case Play:
                _charRepo.Tick(dt);
                _projRepo.Tick(dt);
                _contRepo.Tick(dt);
                break;
        }
    }

    public void OnChangingState(GameState state)
    {
        _state = state;

        Print($"Текущее состояние сервера: {state}");
    }
}