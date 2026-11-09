using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Services;

public class InputSynchronizerService(
    InputSynchronizerRepository inputSyncs,
    CameraPivotControllerRepository pivots,
    long myPeer) : ITickable
{
    private CameraPivotController _cachedPivot;
    private InputSynchronizer _cachedSync;

    public void Tick(float delta)
    {
        bool isJumpPressed = Input.IsActionJustPressed("action_jump");
        bool isPressed = Input.IsActionPressed("action_fire");
        bool k1 = Input.IsActionPressed("action_key1");
        bool k2 = Input.IsActionPressed("action_key2");
        bool k3 = Input.IsActionPressed("action_key3");
        bool k4 = Input.IsActionPressed("action_key4");
        Vector2 wasd = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        if (_cachedPivot == null) 
            if (!pivots.TryGet(myPeer, out _cachedPivot))
                return;
        
        if (_cachedSync == null) 
            if (!inputSyncs.TryGet(myPeer, out _cachedSync))
                return;

        Vector3 mouseDir = GetMouseDirectionIn3D();
        
        _cachedSync.WasdDirection = wasd;
        _cachedSync.SetButtons(isPressed, k1, k2, k3, k4, isJumpPressed);
        _cachedSync.MouseDirection = mouseDir;
    }

    private Vector3 GetMouseDirectionIn3D()
    {
        Vector3 worldLookDirection = _cachedPivot.GlobalTransform.Basis.Z.Normalized();

        return worldLookDirection;
    }
}