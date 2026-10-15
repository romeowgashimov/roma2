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

        //Пока все будут в первой команде
        _playerRepos.AddPlayer(senderId, Shared.Data.TeamType.FirstTeam);

        _networkBridge.RpcId(senderId, nameof(_networkBridge.OnApprovedJoin));

        Print($"{senderId} успешно вошёл в игру");

        if (_playerRepos.PlayersCount == _requiredPlayers) 
        {
            Print($"Все игроки вошли");
            ChangeState?.Invoke(GameState.WaitChoices);
        }
    }
}