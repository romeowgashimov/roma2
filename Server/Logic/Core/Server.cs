using System;
using Godot;
using Godot.Collections;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;
using static Godot.GD;

namespace ROMA2.Server.Logic.Core;

public partial class Server : Node
{
    [Export] private int _serverPort = 7070;
    [Export] private int _requiredPlayers = 1;
    [Export] private NetworkBridge _networkBridge;
    
    private PlayerRepository _playerRepos;

    public event Action<GameState> ChangeState;

    public void Init(PlayerRepository playerRepos)
    {
        _networkBridge.RequestJoin += OnRequestingJoin;
        _playerRepos = playerRepos;
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

        if (_playerRepos.PlayersCount > _requiredPlayers) 
        {
            Print($"Запрос на вход от {senderId} отклонён, так как все места заняты");
            return;
        }

        // Тесты
        //TeamType team = _playerRepos.PlayersCount == 0 ? TeamType.FirstTeam : TeamType.SecondTeam;
        _playerRepos.AddPlayer(senderId, TeamType.FirstTeam);

        _networkBridge.RpcId(senderId, nameof(_networkBridge.OnApprovedJoin));

        Print($"{senderId} успешно вошёл в игру");

        if (_playerRepos.PlayersCount == _requiredPlayers) 
        {
            Print($"Все игроки вошли");
            ChangeState?.Invoke(GameState.WaitChoices);

            Dictionary data = new();
            TeamType team = TeamType.FirstTeam;
            // 2 для тестов
            for (int i = 0; i < 2; ++i) 
                data[i] = _playerRepos.GetPlayer(i, team);
            // 0 тоже для тестов
            team = TeamType.SecondTeam;
            for (int i = 2; i < 0; ++i)
                data[i] = _playerRepos.GetPlayer(i, team);
            
            _networkBridge.Rpc(nameof(_networkBridge.OnAllPlayersConnected), data);
        }
    }
}