using Friflo.Engine.ECS;
using Godot;
using Godot.Collections;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Core;

public class PreGameInitializationService
{
    private PlayerRepository _playerRepos;
    private SpawnConfiguration _spawnConfig;
    private CharacterFactory _factory;
    private EntityStore _store;

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

        for (int i = 0; i < playersCount; ++i)
        {
            var (peer, team) = _playerRepos.GetPlayer(i);
            int characterId = _playerRepos.GetCharacterId(peer);
            
            Dictionary data = [];
            data.Add("characterId", characterId);

            Node3D character = _factory.Create(data);

            character.Position = _spawnConfig.GetSpawnPosition(team, i);

            Entity playerEntity = _store.CreateEntity(
                new PlayerPeer { Value = peer },
                new Team { Value = team });
        }
    }
}