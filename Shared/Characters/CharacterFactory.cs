using Friflo.Engine.ECS;
using Godot;
using Godot.Collections;
using ROMA2.Shared.Configurations;

namespace ROMA2.Shared.Characters;
public class CharacterFactory
{
    private CharactersConfiguration _config;
    private MultiplayerSpawner _spawner;
    private EntityStore _store;

    public CharacterFactory(CharactersConfiguration config, MultiplayerSpawner spawner, EntityStore store)
    {
        _config = config;
        _spawner = spawner;
        _store = store;
        
        _spawner.SpawnFunction = Callable.From<Dictionary, Character>(Spawn);
    }

    public Character Create(Dictionary data)
    {
        return (Character)_spawner.Spawn(data);
    }

    private Character Spawn(Dictionary data)
    {
        int instId = (int)data["characterId"];
        int entityId = (int)data["entityId"];
        long ownerPeer = (long)data["ownerPeer"];

        string path = _config.GetChampionPath(instId);
        string name = _config.GetChampionName(instId);
        
        var character = GD.Load<PackedScene>(path).Instantiate<Character>();

        character.Name = $"{name}_{entityId}"; 

        character.EntityId = entityId;
        character.OwnerPeer = ownerPeer;

        character.Init(_store);

        return character;
    }
}