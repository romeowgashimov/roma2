using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Data;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Systems;

public class InputSynchronizationSystem : QuerySystem<InputSynchronizerReference>
{
    private InputSynchronizerRepository _inputSyncs;
    private CameraPivotControllerRepositoty _pivots;
    private CameraPivotController _cachedPivot;
    private InputSynchronizer _cachedSync;

    public InputSynchronizationSystem(                  
        InputSynchronizerRepository inputSyncs,      
        CameraPivotControllerRepositoty pivots)
    {
        _inputSyncs = inputSyncs;
        _pivots = pivots;

        Filter.AllTags(Tags.Get<LocalPlayerTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref InputSynchronizerReference syncRef, Entity entity) =>
        {
            long ownerPeer = syncRef.OwnerPeer;

            bool isJumpPressed = Input.IsActionJustPressed("action_jump");
            bool isPressed = Input.IsActionPressed("action_fire");
            bool k1 = Input.IsActionPressed("action_key1");
            bool k2 = Input.IsActionPressed("action_key2");
            bool k3 = Input.IsActionPressed("action_key3");
            bool k4 = Input.IsActionPressed("action_key4");
            Vector2 wasd = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            
            
            Vector3 mouseDir = GetMouseDirectionIn3D(ownerPeer);
            
            _cachedSync ??= _inputSyncs.Get(ownerPeer);

            _cachedSync.WasdDirection = wasd;
            _cachedSync.SetButtons(isPressed, k1, k2, k3, k4, isJumpPressed);
            _cachedSync.MouseDirection = mouseDir;
        });
    }

    private Vector3 GetMouseDirectionIn3D(long ownerPeer)
    {
        _cachedPivot ??= _pivots.Get(ownerPeer);

        Vector3 worldLookDirection = _cachedPivot.GlobalTransform.Basis.Z.Normalized();

        return worldLookDirection;
    }
}
