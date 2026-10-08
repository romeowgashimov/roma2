using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared;
using static Godot.GD;
using static ROMA2.Server.Logic.Core.GameState;

namespace ROMA2.Server.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _networkBridge;
    [Export] private Network.Server _server;
    [Export] private GameState _state = WaitPlayers;
    private CharacterSelectionService _charSelectService;

    public override void _Ready()
    {
        ServerProperties properties = new();
        _charSelectService = new();

        _server.Init(properties);
        _server.ChangeState += OnChangingState;

        _charSelectService.Init(properties, _networkBridge);
        _charSelectService.ChangeState += OnChangingState;

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
        }
    }

    public void OnChangingState(GameState state)
    {
        _state = state;
    }
}