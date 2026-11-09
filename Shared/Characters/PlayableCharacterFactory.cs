using Godot;
using Godot.Collections;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Shared.Characters;

public abstract class PlayableCharacterFactory
{
    private CharacterRepository _repository;
    protected PlayableCharacterConfiguration _config;
    protected MultiplayerSpawner _spawner;
    private ProjectileExtendedFactory _projFactory;

    public PlayableCharacterFactory(
        PlayableCharacterConfiguration config, 
        MultiplayerSpawner spawner, 
        CharacterRepository repository,
        ProjectileExtendedFactory projFactory)
    {
        _config = config;
        _spawner = spawner;
        _repository = repository;
        _projFactory = projFactory;
        
        _spawner.SpawnFunction = Callable.From<Dictionary, Character>(Spawn);
    }

    protected virtual Character Spawn(Dictionary data)
    {
        int instId = (int)data["characterId"];
        int reference = (int)data["reference"];
        long ownerPeer = (long)data["ownerPeer"];

        string path = _config.GetCharacterPath(instId);
        string name = _config.GetCharacterName(instId);
        
        var character = GD.Load<PackedScene>(path).Instantiate<Character>();

        character.Name = $"{name}_{ownerPeer}"; 
        character.Reference = reference;
        character.OwnerPeer = ownerPeer;

        character.Init(_projFactory);

        _repository.Add(reference, character);

        return character;
    }
}