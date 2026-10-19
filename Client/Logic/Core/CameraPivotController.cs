using Godot;

namespace ROMA2.Client.Logic.Core;

public partial class CameraPivotController : Node3D
{
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float TwistInput; // Поворот влево/вправо (Y)
    [Export] public float PitchInput; // Поворот вверх/вниз (X)

    // Ограничения для камеры по вертикали (в радианах)
    [Export] public float PitchMin = -Mathf.Pi / 3; // ~ -60 градусов
    [Export] public float PitchMax = Mathf.Pi / 4;  // ~ 45 градусов

    public void Init()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
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
        Rotation = new Vector3(PitchInput, TwistInput, Rotation.Z);
    }
}
