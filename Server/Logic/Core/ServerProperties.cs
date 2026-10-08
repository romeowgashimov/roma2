using System.Collections.Generic;

namespace ROMA2.Server.Logic.Core;

public class ServerProperties
{
    private List<long> _playersIds = [10];

    public int PlayersCount { get => _playersIds.Count; }

    public void AddPlayer(long peerId)
    {
        _playersIds.Add(peerId);
    }

    public long GetPlayer(int id)
    {
        return _playersIds[id];
    }
}