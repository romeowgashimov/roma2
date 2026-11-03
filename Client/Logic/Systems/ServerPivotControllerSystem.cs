using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Data;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Systems;

public class ServerPivotControllerSystem : QuerySystem<CameraPivotControllerReference>
{
    private const float LerpSpeed = 30f; 

    private CameraPivotControllerRepositoty _repository;
    private InputSynchronizerRepository _inputSyncs;
    private Vector3 _allyMouseDirection;
    
    public ServerPivotControllerSystem(
        CameraPivotControllerRepositoty repository, 
        InputSynchronizerRepository inputSyncs)
    {
        _repository = repository;
        _inputSyncs = inputSyncs;

        Filter.AllTags(Tags.Get<NetworkPlayerTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CameraPivotControllerReference controllerRef, Entity entity) =>
        {
            long allyPeer = controllerRef.OwnerPeer;

            var pivotController = _repository.Get(allyPeer);
            var sync = _inputSyncs.Get(allyPeer);

            if (sync.MouseDirection.Equals(Vector3.Zero)) return;

            float deltaTime = (float)pivotController.GetProcessDeltaTime();

            _allyMouseDirection = _allyMouseDirection.Lerp(
                -sync.MouseDirection, 
                LerpSpeed * (float)deltaTime);

            pivotController.Run(_allyMouseDirection);
        });
    }
}
