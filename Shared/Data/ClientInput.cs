using Godot;

namespace ROMA2.Shared.Data;

public readonly record struct ClientInput(
    Vector3 MouseDirection,
    Vector2 WasdDirection,
    int ActionButtonsMask)
{
    public bool IsPressed => (ActionButtonsMask & (1 << 0)) != 0;
    public bool IsKey1Pressed => (ActionButtonsMask & (1 << 1)) != 0;
    public bool IsKey2Pressed => (ActionButtonsMask & (1 << 2)) != 0;
    public bool IsKey3Pressed => (ActionButtonsMask & (1 << 3)) != 0;
    public bool IsKey4Pressed => (ActionButtonsMask & (1 << 4)) != 0;
    public bool IsJumped => (ActionButtonsMask & (1 << 5)) != 0;
}