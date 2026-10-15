using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;
using static Godot.GD;
using static ROMA2.Server.Logic.Core.GameState;

namespace ROMA2.Server.Logic.Core;

public partial class EntryPoint : Node
{
    [ExportGroup("Network")]
    [Export] private NetworkBridge _networkBridge;
    [Export] private Network.Server _server;
    [Export] private GameState _state = WaitPlayers;

    [ExportGroup("Character Factory")]
    [Export] private MultiplayerSpawner _charSpawner;

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

    public override void _Ready()
    {
        EntityStore store = new();
        SpawnConfiguration spawnConfig = new(
            [_fFirstPosition, _fSecondPosition, _fThirdPosition, _fFourthPosition, _fFifthPosition], 
            [_sFirstPosition, _sSecondPosition, _sThirdPosition, _sFourthPosition, _sFifthPosition]);
        PlayerRepository playerRepos = new();
        CharactersConfiguration config = new(true);
        CharacterFactory charFactory = new(config, _charSpawner);

        _charSelectService = new(playerRepos, _networkBridge);
        _charSelectService.ChangeState += OnChangingState;

        _initService = new(playerRepos, spawnConfig, store, charFactory);
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
                break;
        }
    }

    public void OnChangingState(GameState state)
    {
        _state = state;

        Print($"Текущее состояние сервера: {state}");
    }
}