using Godot;

namespace ROMA2.Shared.Characters;

public abstract partial class Aurora : Character
{
    [Export] public int CutCount;
    public int MaxCutCount = 4;
}