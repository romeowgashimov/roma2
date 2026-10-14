using Godot;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public class CharacterSelectionService
{
    private NetworkBridge _bridge;
    private int _selectedCharId = int.MaxValue;

    public CharacterSelectionService(NetworkBridge bridge)
    {
        _bridge = bridge; 
    }

    public void OnCharacterSelecting()
    {
        if (_selectedCharId == int.MaxValue) return;
        
        GD.Print("Client");
        _bridge.Rpc(nameof(_bridge.OnSelectedCharacter), _selectedCharId);
    }

    public void OnChangingSelection(int characterId)
    {
        _selectedCharId = characterId;
    }
}