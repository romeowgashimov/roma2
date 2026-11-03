using Friflo.Engine.ECS;
using Godot;
using Godot.Collections;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;

namespace ROMA2.Client.Logic.Characters;

public class ClientPlayableCharFactory(
    CharactersConfiguration config, 
    MultiplayerSpawner spawner, 
    EntityStore store, 
    CameraPivotControllerRepositoty pivotRepository) : PlayableCharacterFactory(config, spawner, store)
{
    protected override Character Spawn(Dictionary data)
    {
        var character = base.Spawn(data);

        long ownerPeer = (long)data["ownerPeer"];
        
        if (character is IClientPlayableCharacter playable)
        {
            pivotRepository.Add(ownerPeer, playable.PivotController);

            playable.InitClient(_store);
        }

        return character;
    }
}