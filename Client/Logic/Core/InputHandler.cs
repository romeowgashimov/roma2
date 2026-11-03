using System;
using Godot;

namespace ROMA2.Client.Logic.Core;

public partial class InputHandler : Node
{
    public event Action<InputEvent> HandleInput;

    public override void _UnhandledInput(InputEvent @event)
    {
        HandleInput?.Invoke(@event);
    }
}