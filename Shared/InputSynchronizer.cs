using Godot;

namespace ROMA2.Shared;

public partial class InputSynchronizer : MultiplayerSynchronizer
{
    [Export] public Vector3 MouseDirection;
    [Export] public bool IsClicked;
} 