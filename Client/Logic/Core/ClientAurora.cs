using Friflo.Engine.ECS;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public partial class ClientAurora : Aurora
{
    [Export] private CameraPivotController _cameraPivotController;
    [Export] private Camera3D _camera;
    [Export] private AnimationPlayer _animPlayer;

    public override void Init(EntityStore store)
    {
        long ownerPeer = store
                .GetUniqueEntity("MyPeer")
                .GetComponent<PlayerPeer>()
                .Value;

        store
            .CreateEntity(
                new CharacterReference { Value = this }, 
                new AnimationPlayerReference { Value = _animPlayer},
                new AnimationStateComponent { Previous = AnimationState.Idle})
            .AddTag<InterpolateTag>();

        // подумать, так как союзники должны иметь возможность смотреть за другими союзниками
        if (OwnerPeer != ownerPeer) return;

        store.CreateEntity(new CameraReference { Value = _camera }, new UniqueEntity("Camera"));
        store.CreateEntity(new CameraPivotControllerReference { Value = _cameraPivotController }, new UniqueEntity("CameraController"));
        _cameraPivotController.Init();

        _camera.Current = true;
    }
}