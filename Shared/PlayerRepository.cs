using System.Collections.Generic;
using System.Linq;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Shared;

public class PlayerRepository
{
    private List<long> _playersIds = [];
    // key - player peer, int - character id
    private Dictionary<long, int> _pickedCharacters = [];
    // Уже загруженные объекты персонажей
    private Dictionary<long, Node> _initCharacters = [];
    private Dictionary<long, TeamType> _teams = [];

    public int PlayersCount { get => _playersIds.Count; }

    public void AddPlayer(long peerId, TeamType team)
    {
        _playersIds.Add(peerId);
        _teams[peerId] = team;
    }

    public (long, TeamType) GetPlayer(int id)
    {
        long peer = _playersIds[id];
        _teams.TryGetValue(peer, out TeamType team);
        
        return (peer, team);
    }

    public int GetPlayerId(long playerPeer)
    {
        return _playersIds.FindIndex(x => x == playerPeer);
    }

    public bool IsCharacterTaken(long playerPeer, int characterId) 
    {
        // Условие пройдёт, если игрок уже выбрал персонажа или такой персонаж уже выбран кем-то другим
        return _pickedCharacters.Any(x => x.Key == playerPeer || x.Value == characterId);
    }

    public bool TryAddCharacterId(long playerPeer, int characterId)
    {
        return _pickedCharacters.TryAdd(playerPeer, characterId);
    }

    public int GetCharacterId(long peer)
    {
        return _pickedCharacters[peer];
    }
}