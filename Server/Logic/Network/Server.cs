using System;
using Godot;
using ROMA2.Server.Logic.Core;
using ROMA2.Shared;
using static Godot.GD;

namespace ROMA2.Server.Logic.Network;

public partial class Server : Node
{
    [Export] private int _serverPort = 7070;
    [Export] private int _requiredPlayers = 1;
    [Export] private NetworkBridge _networkBridge;
    private ServerProperties _properties;

    public event Action<GameState> ChangeState;

    public void Init(ServerProperties properties)
    {
        _networkBridge.RequestJoin += OnRequestingJoin;
        _properties = properties;
    }

    public void Start()
    {
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateServer(_serverPort, _requiredPlayers);

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

        if (_properties.PlayersCount >= _requiredPlayers) return;

        _properties.AddPlayer(senderId);

        _networkBridge.RpcId(senderId, nameof(_networkBridge.OnApprovedJoin));

        if (_properties.PlayersCount == _requiredPlayers) 
            ChangeState?.Invoke(GameState.WaitChoices);
    }
}