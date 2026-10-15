using Godot;
using static Godot.GD;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public class WorldFactory
{
    private NetworkBridge _bridge;
    private Node _root;
    private string _gameWorldPath;

    public WorldFactory(NetworkBridge bridge, Node root, string gameWorldPath)
    {
        _bridge = bridge;
        _root = root;
        _gameWorldPath = gameWorldPath;
    }

    public bool TryCreate(out GameWorldEntryPoint gameWorld)
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

        gameWorld.Init(_bridge);

        return true;
    }
}