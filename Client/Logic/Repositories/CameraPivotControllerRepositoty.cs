using System.Collections.Generic;
using ROMA2.Client.Logic.Controllers;

namespace ROMA2.Client.Logic.Repositories;

public class CameraPivotControllerRepository
{
    private Dictionary<long, CameraPivotController> _repository = [];

    public void Add(long ownerPeer, CameraPivotController controller)
    {
        _repository[ownerPeer] = controller;
    }

    public bool TryGet(long ownerPeer, out CameraPivotController pivot)
    {
        return _repository.TryGetValue(ownerPeer, out pivot);
    }

    public void Remove(long ownerPeer)
    {
        _repository.Remove(ownerPeer);
    }
}