using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace ROMA2.Shared.Systems;

public class EndSimulationCommandBufferSystem : QuerySystem
{
    private CommandBuffer _commandBuffer;

    public CommandBuffer EndSimulationCommandBuffer => _commandBuffer;

    protected override void OnAddStore(EntityStore store)
    {
        _commandBuffer = store.GetCommandBuffer();
        _commandBuffer.ReuseBuffer = true; 
    }

    protected override void OnUpdate()
    {
        _commandBuffer.Playback();
    }
}
