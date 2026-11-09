using static Godot.GD;
using System;
using ROMA2.Server.Logic.Core;
using ROMA2.Shared.Repositories;
using ROMA2.Shared.Configurations;
using ROMA2.Server.Logic.Characters;
using ROMA2.Server.Logic.Repositories;

namespace ROMA2.Server.Logic.Services;

public class PreGameInitializationService
{
    private PlayerRepository _playerRepos;
    private SpawnConfiguration _spawnConfig;
    private ServerPlayableCharFactory _factory;
    private ServerInputSyncFactory _inputSyncFactory;
    private PlayerControllerRepository _playerControllerRepo;

    public event Action<GameState> ChangeState;

    public PreGameInitializationService(
        PlayerRepository playerRepos, 
        SpawnConfiguration spawnConfig, 
        ServerPlayableCharFactory factory,
        ServerInputSyncFactory inputSyncFactory,
        PlayerControllerRepository playerControllerRepo)
    {
        _playerRepos = playerRepos;
        _spawnConfig = spawnConfig;
        _factory = factory;
        _inputSyncFactory = inputSyncFactory;
        _playerControllerRepo = playerControllerRepo;
    }

    public void Run()
    {
        int playersCount = _playerRepos.PlayersCount;

        Print($"Инициализация игры начата. Всего игроков: {playersCount}");

        for (int i = 0; i < playersCount; ++i)
        {
            var (peer, team) = _playerRepos.GetPlayer(i);
            int characterId = _playerRepos.GetCharacterId(peer);

            var character = _factory.Create(characterId, peer);

            character.Position = _spawnConfig.GetSpawnPosition(team, i);

            var input = _inputSyncFactory.Create(peer);

            _playerControllerRepo.Add(new PlayerController(input, character));
        }

        ChangeState?.Invoke(GameState.Play);
    }
}