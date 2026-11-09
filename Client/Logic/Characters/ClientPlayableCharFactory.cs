using Godot;
using Godot.Collections;
using ROMA2.Client.Logic.Core;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Characters;

public class ClientPlayableCharFactory(
    PlayableCharacterConfiguration config, 
    MultiplayerSpawner spawner, 
    CharacterRepository repository, 
    CameraPivotControllerRepository pivotRepository,
    InputHandler inputHandler,
    ProjectileExtendedFactory projFactory) : PlayableCharacterFactory(config, spawner, repository, projFactory)
{
    protected override Character Spawn(Dictionary data)
    {
        var character = base.Spawn(data);

        if (character is IClientPlayableCharacter playable)
        {
            pivotRepository.Add(character.OwnerPeer, playable.PivotController);

            if (character.OwnerPeer == GlobalConfiguration.MyPeer)
                playable.PivotController.Init(inputHandler);
        }

        return character;
    }
}