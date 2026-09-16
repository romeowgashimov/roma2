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

    public override void _Ready()
    {
        _server.Init();
        _server.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_state)
        {
            case WaitPlayers:
                break;
        }
    }
}