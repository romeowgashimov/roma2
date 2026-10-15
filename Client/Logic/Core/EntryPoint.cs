using Godot;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _bridge;
    [Export] private Network.Client _client;
    [Export] private StartView _startView;
    [Export] private string _gameWorldScenePath = "res://Client/Scenes/game_world.tscn";

    public override void _Ready()
    {
        WorldFactory factory = new(_bridge, this, _gameWorldScenePath);
        
        _client.Init(factory);
        _startView.Init();
    }

    public override void _Process(double delta)
    {
        
    }
}