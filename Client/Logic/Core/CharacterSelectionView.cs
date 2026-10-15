using System;
using Godot;

namespace ROMA2.Client.Logic.Core;

public partial class CharacterSelectionView : Node
{
    [Export] private Button _confirmButton;
    [Export] private Node _charactersButtonsRoot;

    private CharacterSelectionViewModel _viewModel;
    private Action[] _actions;
    private Button[] _charButtons;

    public void Init(CharacterSelectionViewModel viewModel)
    {
        _viewModel = viewModel;

        int length = _charactersButtonsRoot.GetChildCount();

        _actions = new Action[length];
        _charButtons = new Button[length];

        // Исключаем кнопку подтверждения выбора 
        for (int i = 0; i < length - 1; ++i)
        {
            int index = i;

            _actions[index] = () => SelectCharacter(index);
            _charButtons[index] = _charactersButtonsRoot.GetChild<Button>(i);

            _charButtons[index].Pressed += _actions[index];
        }

        _viewModel.CharacterSelected += DrawOutline;

        _confirmButton.Pressed += _viewModel.OnPickedCharacter;
    }

    public void SelectCharacter(int id)
    {
        _viewModel.SelectedCharacter = id;
    }

    public void DrawOutline(int prev, int curr)
    {
        //
        
        EnableConfirmButton();
    }

    public void EnableConfirmButton()
    {
        _confirmButton.Disabled = false;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            int length = _charButtons.Length;

            for (int i = 0; i < length; ++i)
            {
                int index = i;

                _charButtons[index].Pressed -= _actions[index];
            }

            _viewModel.CharacterSelected -= DrawOutline;

            _confirmButton.Pressed -= _viewModel.OnPickedCharacter;

            _viewModel = null;
            Array.Clear(_actions);
            Array.Clear(_charButtons);
        }
    }
}