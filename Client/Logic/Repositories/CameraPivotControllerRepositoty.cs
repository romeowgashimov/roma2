using System.Collections.Generic;
using ROMA2.Client.Logic.Controllers;

namespace ROMA2.Client.Logic.Repositories;

public class CameraPivotControllerRepositoty
{
    private Dictionary<long, CameraPivotController> _repository = [];

    public void Add(long ownerPeer, CameraPivotController controller)
    {
        _repository[ownerPeer] = controller;
    }

    public CameraPivotController Get(long ownerPeer)
    {
        return _repository[ownerPeer];
    }

    public void Remove(long ownerPeer)
    {
        _repository.Remove(ownerPeer);
    }
}