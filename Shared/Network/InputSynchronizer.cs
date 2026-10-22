using Godot;

namespace ROMA2.Shared.Network;

public partial class InputSynchronizer : Node
{
    // Направление взгляда/мыши (если это 3D-вектор на плоскости, тоже можно подумать о Vector2)
    [Export] public Vector3 MouseDirection { get; set; }
    
    // Направление движения (X и Z оси), экономим на векторе Y
    [Export] public Vector2 WasdDirection { get; set; }

    // Все ваши кнопки в одной переменной (битовая маска)
    [Export] public int ActionButtonsMask { get; set; }

    // Удобные свойства-хелперы для проверки кнопок в коде персонажа
    public bool IsPressed => (ActionButtonsMask & (1 << 0)) != 0;
    public bool IsKey1Pressed => (ActionButtonsMask & (1 << 1)) != 0;
    public bool IsKey2Pressed => (ActionButtonsMask & (1 << 2)) != 0;
    public bool IsKey3Pressed => (ActionButtonsMask & (1 << 3)) != 0;
    public bool IsKey4Pressed => (ActionButtonsMask & (1 << 4)) != 0;
    public bool IsJumped => (ActionButtonsMask & (1 << 5)) != 0;

    // Метод для записи ввода на стороне клиента
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
}
