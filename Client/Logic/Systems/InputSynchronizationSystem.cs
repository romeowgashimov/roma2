using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Systems;

public class InputSynchronizationSystem(EntityStore store) : QuerySystem<InputSynchronizerReference>
{
    private EntityStore _store = store;

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref InputSynchronizerReference syncRef, Entity entity) =>
        {
            bool isJumpPressed = Input.IsActionJustPressed("action_jump");
            bool isPressed = Input.IsActionPressed("action_fire");
            bool k1 = Input.IsActionPressed("action_key1");
            bool k2 = Input.IsActionPressed("action_key2");
            bool k3 = Input.IsActionPressed("action_key3");
            bool k4 = Input.IsActionPressed("action_key4");
            Vector2 wasd = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            Vector3 mouseDir = GetMouseDirectionIn3D();
            
            var syncNode = syncRef.Value;

            // Обязательно проверяем жива ли еще Godot-нода на сцене
            if (syncNode == null)
                return;

            syncNode.WasdDirection = wasd;
            syncNode.SetButtons(isPressed, k1, k2, k3, k4, isJumpPressed);
            syncNode.MouseDirection = mouseDir;
        });
    }

    private Vector3 GetMouseDirectionIn3D()
    {
        Camera3D camera = _store
                .GetUniqueEntity("Camera")  
                .GetComponent<CameraReference>()
                .Value;
        
        if (camera == null) return Vector3.Zero;

        Vector3 worldLookDirection = -camera.GlobalTransform.Basis.Z.Normalized();

        return worldLookDirection;
    }
}
