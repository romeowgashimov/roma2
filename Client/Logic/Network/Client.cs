using Godot;
using ROMA2.Shared;
using static Godot.GD;

namespace ROMA2.Client.Logic.Network;

public partial class Client : Node
{
    [Export] private string _gameWorldScenePath = "res://Client/Scenes/GameWorld.tscn";
    [Export] private string _serverIp = "127.0.0.1";
    [Export] private int _serverPort = 7070;
    [Export] private NetworkBridge _networkBridge;

    // Сделать очистку событий
    public void Init()
    {
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;

        if (_networkBridge != null)
            _networkBridge.ApproveJoin += OnApprovedJoin;
    }

    public void OnConnecting()
    {
        Print($"Попытка подключения к серверу...");

        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateClient(_serverIp, _serverPort);

        if (error != Error.Ok)
        {
            PrintErr($"Ошибка инициализации сети: {error}");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
    }

    private void OnConnectedToServer()
    {
        Print($"Соединение установлено. Отправка запроса на вход...");

        _networkBridge.Rpc(nameof(_networkBridge.OnRequestingJoin));
    }

    private void OnConnectionFailed()
    {
        PrintErr($"Не удалось подключиться к серверу.");
        
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnApprovedJoin()
    {
        PackedScene gameWorldScene = Load<PackedScene>(_gameWorldScenePath);

        if (gameWorldScene == null)
        {
            PrintErr($"Не удалось загрузить ресурс сцены по пути: {_gameWorldScenePath}");
            return;
        }

        Node gameWorldInstance = gameWorldScene.Instantiate();

        AddChild(gameWorldInstance);
    }
}