using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Network;

namespace ROMA2.Shared.Repositories;

public class InputSynchronizerRepository
{
    private Dictionary<long, InputSynchronizer> _inputs = [];

    public void Add(long localId, InputSynchronizer sync)
    {
        _inputs[localId] = sync;
    }

    public InputSynchronizer Remove(long localId)
    {
        var projectile = _inputs[localId];
        _inputs.Remove(localId);
        return projectile;
    }

    public InputSynchronizer Get(long localId)
    {
        return _inputs[localId];
    }

    public MultiplayerSynchronizer GetSynchronizer(long localId)
    {
        return Get(localId).Synchronizer;
    }
}