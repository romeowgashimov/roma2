using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientPlayableCharacter : IClientCharacter
{
    Camera3D Camera { get; }
    CameraPivotController PivotController { get; }

    new void InitClient(EntityStore store)
    {
        ((IClientCharacter)this).InitClient(store);
        
        long myPeer = store
                .GetUniqueEntity("MyPeer")
                .GetComponent<PlayerPeer>()
                .Value;
                
        PivotController.Entity = store.CreateEntity(
            new CameraPivotControllerReference { OwnerPeer = Character.OwnerPeer });
            
        PivotController.Init();

        if (Character.OwnerPeer != myPeer) return;

        PivotController.Entity.AddTag<LocalPlayerTag>();
        Camera.Current = true;
    }
}