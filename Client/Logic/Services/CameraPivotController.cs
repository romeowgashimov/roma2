using Godot;
using ROMA2.Shared.Characters;

namespace ROMA2.Client.Logic.Services;

public partial class CameraPivotController : Node3D
{
    [Export] private Character _character;
    [Export] private Vector3 _offset;

    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float TwistInput; // Поворот влево/вправо (Y)
    [Export] public float PitchInput; // Поворот вверх/вниз (X)

    // Ограничения для камеры по вертикали (в радианах)
    [Export] public float PitchMin = -Mathf.Pi / 3; // ~ -60 градусов
    [Export] public float PitchMax = Mathf.Pi / 4;  // ~ 45 градусов

    public void Init()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        TopLevel = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            TwistInput -= mouseMotion.Relative.X * MouseSensitivity;
            PitchInput += mouseMotion.Relative.Y * MouseSensitivity;
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured 
                ? Input.MouseModeEnum.Visible 
                : Input.MouseModeEnum.Captured;
        }
    }

    public void Run()
    {
        PitchInput = Mathf.Clamp(PitchInput, PitchMin, PitchMax);

        Rotation = new Vector3(PitchInput, TwistInput, 0.0f);

        Position = _character.Position + (Transform.Basis * _offset);
    }
}
