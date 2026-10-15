using System;

namespace ROMA2.Client.Logic.Core;

public class CharacterSelectionViewModel
{
    private CharacterSelectionService _service;
    private int _selectedCharacter;

    public int SelectedCharacter
    {
        get { return _selectedCharacter; }

        set 
        {
            if (_service.IsCharacterTaken(value)) return;
            
            CharacterSelected?.Invoke(_selectedCharacter, value);
            _selectedCharacter = value;
        }
    }

    public event Action<int, int> CharacterSelected;
    public event Action<long, int> CharacterPicked;

    public CharacterSelectionViewModel(CharacterSelectionService service)
    {
        _service = service;

        _service.CharacterPicked += OnCharacterPicked;
    }

    public void OnCharacterPicked(long peer, int id)
    {
        CharacterPicked?.Invoke(peer, id);
    }

    public void OnPickedCharacter()
    {
        _service.SelectCharacter(_selectedCharacter);
    }
}