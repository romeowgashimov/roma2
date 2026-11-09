using Godot;
using Godot.Collections;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Characters;

public class ServerPlayableCharFactory(
    PlayableCharacterConfiguration config, 
    MultiplayerSpawner spawner, 
    CharacterRepository repository, 
    ProjectileExtendedFactory projFactory) : PlayableCharacterFactory(config, spawner, repository, projFactory)
{
    public Character Create(int charId, long ownerPeer)
    {
        Dictionary data = new()
        {
            { "characterId", charId },
            { "reference", _config.CharactersCount },
            { "ownerPeer", ownerPeer }
        };
        return (Character)_spawner.Spawn(data);
    }
}