using Godot;
using ROMA2.Shared;
using static Godot.GD;

namespace ROMA2.Server.Logic.Network;

public partial class Server : Node
{
    [Export] private int _serverPort = 7070;
    [Export] private int _maxPlayers = 10;
    [Export] private NetworkBridge _networkBridge;

    public void Init()
    {
        _networkBridge.RequestJoin += OnRequestingJoin;
    }

    public void Start()
    {
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateServer(_serverPort, _maxPlayers);

        if (error != Error.Ok)
        {
            PrintErr($"Не удалось запустить сервер: {error}");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        Print($"Выделенный сервер запущен и слушает порт {_serverPort}...");
    }

    private void OnRequestingJoin(long senderId)
    {
        Print($"Получен запрос на вход от ID: {senderId}");

        _networkBridge.RpcId(senderId, nameof(_networkBridge.OnApprovedJoin));
    }
}