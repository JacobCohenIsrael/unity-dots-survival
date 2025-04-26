using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, moveSpeed, physicsVelocity) 
                 in SystemAPI.Query<
                     RefRW<LocalTransform>, 
                     RefRO<MoveSpeed>, 
                     RefRW<PhysicsVelocity>>())
        {
            var targetPosition = localTransform.ValueRO.Position + new float3(10, 0, 0);
            var moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);
            
            localTransform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.Value;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
