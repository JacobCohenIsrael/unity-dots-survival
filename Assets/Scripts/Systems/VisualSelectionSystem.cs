using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct VisualSelectionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var selection in SystemAPI.Query<RefRO<Selection>>().WithDisabled<Selection>())
        {
            var visualEntity = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.VisualEntity);
            visualEntity.ValueRW.Scale = 0f;
        }
        
        foreach (var selection in SystemAPI.Query<RefRO<Selection>>())
        {
            var visualEntity = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.VisualEntity);
            visualEntity.ValueRW.Scale = selection.ValueRO.ShowScale;
        }
    }
}
