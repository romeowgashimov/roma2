using Friflo.Engine.ECS;
using Godot;
using Godot.Collections;
using ROMA2.Shared.Data;

namespace ROMA2.Shared;
public class CharacterFactory
{
    private CharactersConfig _config;
    private MultiplayerSpawner _spawner;
    private Node _characterRoot;

    public CharacterFactory(CharactersConfig config, MultiplayerSpawner spawner, Node characterRoot)
    {
        _config = config;
        _spawner = spawner;
        _characterRoot = characterRoot;
        //GD.Print(characterRoot.Name);
        
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
        
        var character = GD.Load<PackedScene>(path).Instantiate<Character>();

        GD.Print(path);

        character.Name = $"Character_{entityId}"; 

        character.EntityId = entityId;

        return character;
    }
}