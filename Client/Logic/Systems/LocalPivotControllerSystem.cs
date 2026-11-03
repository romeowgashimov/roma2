using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using ROMA2.Client.Logic.Data;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Client.Logic.Controllers;

namespace ROMA2.Client.Logic.Systems;

public class LocalPivotControllerSystem : QuerySystem<CameraPivotControllerReference>
{
    private CameraPivotControllerRepositoty _repository;
    private CameraPivotController _cachedPivot;

    public LocalPivotControllerSystem(CameraPivotControllerRepositoty repository)
    {
        _repository = repository;

        Filter.AllTags(Tags.Get<LocalPlayerTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CameraPivotControllerReference controllerRef, Entity entity) =>
        {
            _cachedPivot ??= _repository.Get(controllerRef.OwnerPeer);

            // Логика получения инпут ивентов остаётся в объекте, 
            // так как если её выносить, то происходит рассинхрон и появляются дёргания
            _cachedPivot.Run();
        });
    }
}
