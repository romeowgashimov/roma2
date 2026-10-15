using System;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public class CharacterSelectionService : IDisposable
{
    private NetworkBridge _bridge;
    private PlayerRepository _playerRepos;

    public event Action<long, int> CharacterPicked; 

    public CharacterSelectionService(NetworkBridge bridge, PlayerRepository playerRepos)
    {
        _bridge = bridge;
        _playerRepos = playerRepos;
    }

    public void SelectCharacter(int characterId)
    {
        if (!_playerRepos.IsCharacterTaken(0, characterId))
            _bridge.Rpc(nameof(_bridge.OnSelectedCharacter), characterId);
    }

    public void OnCharacterPicked(long peer, int characterId)
    {
        if (_playerRepos.TryAddCharacterId(peer, characterId))
            CharacterPicked?.Invoke(peer, characterId);
    }

    public bool IsCharacterTaken(int id)
    {
        return _playerRepos.IsCharacterTaken(0, id);
    }

    public void Dispose()
    {
        CharacterPicked = null;

        GC.SuppressFinalize(this);
    }
}