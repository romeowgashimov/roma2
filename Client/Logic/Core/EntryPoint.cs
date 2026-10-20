using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Systems;
using ROMA2.Client.Logic.Views;
using ROMA2.Shared.Network;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _bridge;
    [Export] private Client _client;
    [Export] private StartView _startView;
    [Export] private string _gameWorldScenePath = "res://Client/Scenes/game_world.tscn";

    private EntityStore _store;
    private SystemGroup _group;
    private SystemRoot _root;

    public override void _Ready()
    {
        _store = new();
        _group = new("groups")
        {
            new InputSynchronizationSystem(_store),
            new TransformInterpolationSystem(),
            new AnimationSystem()
        };
        _root = new(_store)
        {
            _group
        };

        WorldFactory factory = new(_bridge, this, _gameWorldScenePath, _store, _group);
        
        _client.Init(factory);
        _startView.Init();
    }

    public override void _Process(double delta)
    {
        _root.Update(new());
        
    }
}