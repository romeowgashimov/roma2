using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Server.Logic.Services;
using ROMA2.Server.Logic.Systems;
using ROMA2.Shared.Characters;
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
    [Export]private Node _inputRoot;

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

    private SystemRoot _root;

    public override void _Ready()
    {
        EntityStore store = new();
        _root = new(store)
        {
            new AnimationStateResetSystem(),
            new MovementSystem(),
            new AnimationStateRegistrationSystem()
        };

        SpawnConfiguration spawnConfig = new(
            [_fFirstPosition, _fSecondPosition, _fThirdPosition, _fFourthPosition, _fFifthPosition], 
            [_sFirstPosition, _sSecondPosition, _sThirdPosition, _sFourthPosition, _sFifthPosition]);
        long myPeer = _server.Multiplayer.GetUniqueId();
        PlayerRepository playerRepos = new() { MyPeer = myPeer };
        CharactersConfiguration config = new(isServer: true);
        CharacterFactory charFactory = new(config, _charSpawner, store);
        InputSynchronizerFactory inputSyncFactory = new(_inputRoot, myPeer, true);

        _charSelectService = new(playerRepos, _networkBridge);
        _charSelectService.ChangeState += OnChangingState;

        _initService = new(playerRepos, spawnConfig, store, charFactory, _networkBridge, inputSyncFactory);
        _initService.ChangeState += OnChangingState;

        _server.Init(playerRepos);
        _server.ChangeState += OnChangingState;

        _server.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_state)
        {
            case WaitPlayers:
                break;
            case WaitChoices:
                _charSelectService.Run(delta);
                break;
            case Initialization:
                _initService.Run();
                break;
            case Play:
                _root.Update(new UpdateTick((float)delta, 0));
                break;
        }
    }

    public void OnChangingState(GameState state)
    {
        _state = state;

        Print($"Текущее состояние сервера: {state}");
    }
}