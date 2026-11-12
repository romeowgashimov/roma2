using Godot;
using ROMA2.Client.Logic.Core;
using ROMA2.Shared.Characters;

namespace ROMA2.Client.Logic.Controllers;

public partial class CameraPivotController : Node3D
{
    [Export] private Character _character;
    [Export] private Vector3 _offset;

    [Export] public Camera3D Camera;

    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float TwistInput; // Поворот влево/вправо (Y)
    [Export] public float PitchInput; // Поворот вверх/вниз (X)

    // Ограничения для камеры по вертикали (в радианах)
    [Export] public float PitchMin = -Mathf.Pi / 3; // ~ -60 градусов
    [Export] public float PitchMax = Mathf.Pi / 4;  // ~ 45 градусов

    private InputHandler _inputHandler;

    public void Init(InputHandler inputHandler)
    {
        _inputHandler = inputHandler;

        _inputHandler.HandleInput += CalculateInput;

        Input.MouseMode = Input.MouseModeEnum.Captured;
        TopLevel = true;
        Camera.Current = true;
    }

    public void Tick()
    {
        PitchInput = Mathf.Clamp(PitchInput, PitchMin, PitchMax);

        Position = _character.Position + (Transform.Basis * _offset);

        Rotation = new Vector3(PitchInput, TwistInput, 0.0f);
    }

    public void Tick(Vector3 mouseDirection)
    {
        Position = _character.Position + (Transform.Basis * _offset);

        Basis targetBasis = Basis.LookingAt(mouseDirection, Vector3.Up);

        Rotation = targetBasis.GetEuler();
    } 

    private void CalculateInput(InputEvent @event)
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
}
