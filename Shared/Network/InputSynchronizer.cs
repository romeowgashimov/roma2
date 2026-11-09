using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Network;

public partial class InputSynchronizer : Node
{
    [Export] public Vector3 MouseDirection { get; set; }
    [Export] public Vector2 WasdDirection { get; set; }
    [Export] public int ActionButtonsMask { get; set; }

    [Export] public MultiplayerSynchronizer Synchronizer;

    public bool IsPressed => (ActionButtonsMask & (1 << 0)) != 0;
    public bool IsKey1Pressed => (ActionButtonsMask & (1 << 1)) != 0;
    public bool IsKey2Pressed => (ActionButtonsMask & (1 << 2)) != 0;
    public bool IsKey3Pressed => (ActionButtonsMask & (1 << 3)) != 0;
    public bool IsKey4Pressed => (ActionButtonsMask & (1 << 4)) != 0;
    public bool IsJumped => (ActionButtonsMask & (1 << 5)) != 0;

    public long Reference { get; set; }

    public void SetButtons(bool isPressed, bool k1, bool k2, bool k3, bool k4, bool jump)
    {
        int mask = 0;
        if (isPressed) mask |= 1 << 0;
        if (k1)        mask |= 1 << 1;
        if (k2)        mask |= 1 << 2;
        if (k3)        mask |= 1 << 3;
        if (k4)        mask |= 1 << 4;
        if (jump)      mask |= 1 << 5;
        ActionButtonsMask = mask;
    }

    public ClientInput CreateInput()
    {
        return new ClientInput
        {
            MouseDirection = MouseDirection,
            WasdDirection = WasdDirection,
            ActionButtonsMask = ActionButtonsMask
        };
    }
}
