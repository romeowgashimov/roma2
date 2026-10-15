using Friflo.Engine.ECS;
using Godot.Collections;
using static Godot.GD;
using ROMA2.Shared;
using ROMA2.Shared.Data;
using System;

namespace ROMA2.Server.Logic.Core;

public class PreGameInitializationService
{
    private PlayerRepository _playerRepos;
    private SpawnConfiguration _spawnConfig;
    private CharacterFactory _factory;
    private EntityStore _store;

    public event Action<GameState> ChangeState;

    public PreGameInitializationService(
        PlayerRepository playerRepos, 
        SpawnConfiguration spawnConfig, 
        EntityStore store,
        CharacterFactory factory)
    {
        _playerRepos = playerRepos;
        _spawnConfig = spawnConfig;
        _store = store;
        _factory = factory;
    }

    public void Run()
    {
        int playersCount = _playerRepos.PlayersCount;

        Print($"Инициализация игры начата. Всего игроков: {playersCount}");

        for (int i = 0; i < playersCount; ++i)
        {
            var (peer, team) = _playerRepos.GetPlayer(i);
            int characterId = _playerRepos.GetCharacterId(peer);
            var charEntity = _store.CreateEntity();
            
            Dictionary data = [];
            data.Add("characterId", characterId);
            data.Add("entityId", charEntity.Id);

            Print($"Данные о создании персонажа: {data}");

            var character = _factory.Create(data);

            character.Position = _spawnConfig.GetSpawnPosition(team, i);

            var playerEntity = _store.CreateEntity(
                new PlayerPeer { Value = peer },
                new Team { Value = team },
                new CharacterReference { Value = character });
            

            character.Init(_store);
        }

        ChangeState?.Invoke(GameState.Play);
    }
}