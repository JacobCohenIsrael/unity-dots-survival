using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, unitMovement, physicsVelocity) 
                 in SystemAPI.Query<
                     RefRW<LocalTransform>, 
                     RefRO<UnitMovement>, 
                     RefRW<PhysicsVelocity>>())
        {
            var moveDirection = unitMovement.ValueRO.TargetPosition - localTransform.ValueRO.Position;
            moveDirection = math.lengthsq(moveDirection) > 0f ? math.normalize(moveDirection) : float3.zero;

            localTransform.ValueRW.Rotation = 
                math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotationSafe(moveDirection, math.up()), SystemAPI.Time.DeltaTime * unitMovement.ValueRO.RotationSpeed);
            physicsVelocity.ValueRW.Linear = moveDirection * unitMovement.ValueRO.MoveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
