using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace ROMA2.Client.Logic.Core;

public class CameraPivotControllerSystem : QuerySystem<CameraPivotControllerReference>
{
    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CameraPivotControllerReference controllerRef, Entity entity) =>
        {
            var cameraController = controllerRef.Value;
            cameraController?.Run();
        });
    }
}
