using Godot;
using Godot.Collections;
using ROMA2.Shared.Data;

namespace ROMA2.Shared;
public class CharacterFactory
{
    private CharactersConfiguration _config;
    private MultiplayerSpawner _spawner;

    public CharacterFactory(CharactersConfiguration config, MultiplayerSpawner spawner)
    {
        _config = config;
        _spawner = spawner;
        
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

        string path = _config.GetChampionPath(instId);
        string name = _config.GetChampionName(instId);
        
        var character = GD.Load<PackedScene>(path).Instantiate<Character>();

        character.Name = $"{name}_{entityId}"; 

        character.EntityId = entityId;

        return character;
    }
}