using Friflo.Engine.ECS;
using Godot.Collections;
using static Godot.GD;
using ROMA2.Shared.Data;
using System;
using ROMA2.Server.Logic.Core;
using ROMA2.Shared.Repositories;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Network;

namespace ROMA2.Server.Logic.Services;

public class PreGameInitializationService
{
    private PlayerRepository _playerRepos;
    private SpawnConfiguration _spawnConfig;
    private CharacterFactory _factory;
    private EntityStore _store;
    private NetworkBridge _bridge;
    private InputSynchronizerFactory _inputSyncFactory;

    public event Action<GameState> ChangeState;

    public PreGameInitializationService(
        PlayerRepository playerRepos, 
        SpawnConfiguration spawnConfig, 
        EntityStore store,
        CharacterFactory factory,
        NetworkBridge bridge,
        InputSynchronizerFactory inputSyncFactory)
    {
        _playerRepos = playerRepos;
        _spawnConfig = spawnConfig;
        _store = store;
        _factory = factory;
        _bridge = bridge;
        _inputSyncFactory = inputSyncFactory;
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
            data.Add("ownerPeer", peer);

            Print($"Данные о создании персонажа: {data}");

            var character = _factory.Create(data);

            character.Position = _spawnConfig.GetSpawnPosition(team, i);

            var inputSync = _inputSyncFactory.Create(peer);
            _bridge.RpcId(peer, nameof(_bridge.OnInputSynchronizerCreation));

            var playerEntity = _store.CreateEntity(
                new PlayerPeer { Value = peer },
                new Team { Value = team },
                new CharacterReference { Value = character },
                new InputSynchronizerReference { Value = inputSync },
                new MoveSpeed { Value = 5 },
                new AnimationStateComponent(),
                new MaxHealth { Value = 1000 },
                new CurrentHealth { Value = 1000});
        }

        ChangeState?.Invoke(GameState.Play);
    }
}