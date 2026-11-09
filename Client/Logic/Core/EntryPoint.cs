using Godot;
using ROMA2.Client.Logic.Views;
using ROMA2.Shared.Network;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _bridge;
    [Export] private Client _client;
    [Export] private StartView _startView;
    [Export] private string _gameWorldScenePath = "res://Client/Scenes/game_world.tscn";
    [Export] private InputHandler _inputHandler;

    public override void _Ready()
    {
        WorldFactory factory = new(_bridge, this, _gameWorldScenePath, _inputHandler);
        
        _client.Init(factory);
        _startView.Init();
    }
}