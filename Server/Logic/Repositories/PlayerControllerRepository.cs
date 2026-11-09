using System.Collections.Generic;
using ROMA2.Server.Logic.Core;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Server.Logic.Repositories;

public class PlayerControllerRepository : ITickable
{
    private List<PlayerController> _playerControllers = [];

    public void Tick(float delta)
    {
        foreach (var controller in _playerControllers)
            controller.Tick(delta);
    }

    public void Add(PlayerController controller)
    {
        _playerControllers.Add(controller);
    }
}