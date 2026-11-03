using Godot;
using static Godot.GD;
using ROMA2.Shared;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public class WorldFactory
{
    private NetworkBridge _bridge;
    private Node _root;
    private string _gameWorldPath;
    private EntityStore _store;
    private SystemGroup _group;
    private InputHandler _inputHandler;

    public WorldFactory(
        NetworkBridge bridge, 
        Node root, 
        string gameWorldPath, 
        EntityStore store, 
        SystemGroup group, 
        InputHandler inputHandler)
    {
        _group = group;
        _store = store;
        _bridge = bridge;
        _root = root;
        _gameWorldPath = gameWorldPath;
        _inputHandler = inputHandler;
    }

    public bool TryCreate(Client client, PlayerRepository playerRepository, out GameWorldEntryPoint gameWorld)
    {
        var gameWorldScene = Load<PackedScene>(_gameWorldPath);

        if (gameWorldScene == null)
        {
            PrintErr($"Не удалось загрузить ресурс сцены по пути: {_gameWorldPath}");
            gameWorld = null;
            return false;
        }

        gameWorld = gameWorldScene.Instantiate<GameWorldEntryPoint>();
        
        _root.AddChild(gameWorld);

        gameWorld.Init(_bridge, client, _store, _group, playerRepository, _inputHandler);

        return true;
    }
}