using System;
using Godot;

namespace ROMA2.Client.Logic.Core;

public partial class CharacterSelectionMenu : Node
{
    [Export] private Button _confirmButton;
    [Export] private Node _charactersButtons;
    private CharacterSelectionService _service;
    private Action[] _buttonActions;

    public void Init(CharacterSelectionService service)
    {
        _service = service;

        int length = _charactersButtons.GetChildCount();
        _buttonActions = new Action[length];

        for (int i = 0; i < length - 1; ++i)
        {
            int index = i;

            _buttonActions[i] = () => _service.OnChangingSelection(index);

            _charactersButtons.GetChild<Button>(i).Pressed += _buttonActions[i];
        }

        _confirmButton.Pressed += _service.OnCharacterSelecting;
    }

    /*public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            if (_buttonActions != null)
            {
                int length = _charactersButtons.GetChildCount();
                for (int i = 0; i < length; ++i)
                {
                    _charactersButtons.GetChild<Button>(i).Pressed -= _buttonActions[i];
                }
            }

            _confirmButton.Pressed -= _service.OnCharacterSelecting;
        }
    }*/
}