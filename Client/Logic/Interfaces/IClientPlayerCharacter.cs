using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientPlayerCharacter : IClientCharacter
{
    Camera3D Camera { get; }
    CameraPivotController PivotController { get; }

    new void InitClient(EntityStore store)
    {
        ((IClientCharacter)this).InitClient(store);
        
        long ownerPeer = store
                .GetUniqueEntity("MyPeer")
                .GetComponent<PlayerPeer>()
                .Value;

        // подумать, так как союзники должны иметь возможность смотреть за другими союзниками
        if (Character.OwnerPeer != ownerPeer) return;

        store.CreateEntity(
            new CameraReference { Value = Camera }, 
            new UniqueEntity("Camera"));
        store.CreateEntity(
            new CameraPivotControllerReference { Value = PivotController }, 
            new UniqueEntity("CameraController"));
            
        PivotController.Init();

        Camera.Current = true;
    }
}