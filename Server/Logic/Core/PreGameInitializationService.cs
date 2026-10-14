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

        GD.Print(playersCount);
        for (int i = 0; i < playersCount; ++i)
        {
            var (peer, team) = _playerRepos.GetPlayer(i);
            int characterId = _playerRepos.GetCharacterId(peer);
            var charEntity = _store.CreateEntity();
            
            Dictionary data = [];
            data.Add("characterId", characterId);
            data.Add("entityId", charEntity.Id);

            var character = _factory.Create(data);

            GD.Print(data);

            character.Position = _spawnConfig.GetSpawnPosition(team, i);

            var playerEntity = _store
                .CreateEntity(
                    new PlayerPeer { Value = peer },
                    new Team { Value = team },
                    new CharacterReference { Value = character });
            

            character.Init(_store);
        }
    }
}