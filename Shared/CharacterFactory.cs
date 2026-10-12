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
    private EntityStore _store;

    public CharacterFactory(CharactersConfig config, MultiplayerSpawner spawner, Node characterRoot)
    {
        _config = config;
        _spawner = spawner;
        _characterRoot = characterRoot;
        
        _spawner.SpawnFunction = Callable.From<Dictionary, Node3D>(Spawn);
    }

    public Node3D Create(Dictionary data)
    {
        return (Node3D)_spawner.Spawn(data);
    }

    private Node3D Spawn(Dictionary data)
    {
        int id = (int)data["characterId"];
        string path = _config.GetChampionPath(id);
        
        var character = GD.Load<PackedScene>(path).Instantiate<Node3D>();

        _characterRoot.AddChild(character);

        return character;
    }
}