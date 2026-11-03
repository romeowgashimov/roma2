using System;
using Godot;
using Godot.Collections;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;
using static Godot.GD;

namespace ROMA2.Client.Logic.Core;

public partial class Client : Node
{
    [Export] private string _serverIp = "127.0.0.1";
    [Export] private int _serverPort = 7070;
    [Export] private NetworkBridge _networkBridge;
    
    private WorldFactory _factory;
    private PlayerRepository _playerRepository;

    public Action SuccessfulConnection;

    // Сделать очистку событий
    public void Init(WorldFactory factory)
    {
        _factory = factory;

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;

        if (_networkBridge != null)
        {
            _networkBridge.ApproveJoin += OnApprovedJoin;
            _networkBridge.AllPlayersConnected += OnAllPlayersConnected;
        }
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
        _playerRepository = new() { MyPeer = Multiplayer.GetUniqueId() };

        if (_factory.TryCreate(this, _playerRepository, out var _))
            SuccessfulConnection?.Invoke();
        else 
            PrintErr($"{nameof(Client)} Ошибка создания игрового мира");
    }

    private void OnAllPlayersConnected(Dictionary data)
    {
        TeamType team = TeamType.FirstTeam;
        // 2 для тестов
        for (int i = 0; i < 2; ++i) 
            _playerRepository.AddPlayer((int)data[i], team);
        // 0 тоже для тестов
        team = TeamType.SecondTeam;
        for (int i = 2; i < 0; ++i)
            _playerRepository.AddPlayer((int)data[i], team);
    }
}