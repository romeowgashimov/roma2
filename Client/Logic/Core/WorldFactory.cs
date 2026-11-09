using Godot;
using static Godot.GD;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Core;

public class WorldFactory
{
    private NetworkBridge _bridge;
    private Node _root;
    private string _gameWorldPath;
    private InputHandler _inputHandler;

    public WorldFactory(
        NetworkBridge bridge, 
        Node root, 
        string gameWorldPath, 
        InputHandler inputHandler)
    {
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

        gameWorld.Init(_bridge, client, playerRepository, _inputHandler);

        return true;
    }
}