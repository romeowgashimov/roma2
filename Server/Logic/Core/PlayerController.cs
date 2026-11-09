using ROMA2.Shared.Characters;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Network;

namespace ROMA2.Server.Logic.Core;

public class PlayerController(InputSynchronizer sync, Character character) : ITickable
{
    public void Tick(float delta)
    {
        character.Input = sync.CreateInput();
    }
}