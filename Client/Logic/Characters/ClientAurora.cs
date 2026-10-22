using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Interfaces;
using ROMA2.Shared.Characters;

namespace ROMA2.Client.Logic.Characters;

public partial class ClientAurora : Aurora, IClientPlayerCharacter
{
    [Export] public Camera3D Camera { get; set; }
    [Export] public CameraPivotController PivotController { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }
    [Export] public Character Character { get; set; }

    public override void Init(EntityStore store)
    {
        ((IClientPlayerCharacter)this).InitClient(store);
    }
}