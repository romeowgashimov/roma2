using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Core;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Client.Logic.Services;

public class CameraPivotControllerService(CameraPivotControllerRepository repo) : ITickable
{
    private CameraPivotController _cachedPivot;

    public bool Enabled { get; set; } = true;

    public void Tick(float delta)
    {
        if (!Enabled) return;
         
        if (_cachedPivot == null)
            if (!TryResetMyPivot()) return; 
        
        _cachedPivot.Tick();
    }

    public bool TryResetMyPivot()
    {
        bool success = repo.TryGet(GlobalConfiguration.MyPeer, out _cachedPivot);
        if (success) _cachedPivot.Camera.Current = true;

        return success;
    }
}