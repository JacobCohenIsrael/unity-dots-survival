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
        var job = new UnitMovementJob { deltaTime = SystemAPI.Time.DeltaTime };
        job.ScheduleParallel();        
        // foreach (var (localTransform, unitMovement, physicsVelocity) 
        //          in SystemAPI.Query<
        //              RefRW<LocalTransform>, 
        //              RefRO<UnitMovement>, 
        //              RefRW<PhysicsVelocity>>())
        // {
        //     var moveDirection = unitMovement.ValueRO.TargetPosition - localTransform.ValueRO.Position;
        //     moveDirection = math.lengthsq(moveDirection) > 0f ? math.normalize(moveDirection) : float3.zero;
        //
        //     localTransform.ValueRW.Rotation = 
        //         math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotationSafe(moveDirection, math.up()), SystemAPI.Time.DeltaTime * unitMovement.ValueRO.RotationSpeed);
        //     physicsVelocity.ValueRW.Linear = moveDirection * unitMovement.ValueRO.MoveSpeed;
        //     physicsVelocity.ValueRW.Angular = float3.zero;
        // }
    }
}

[BurstCompile]
public partial struct UnitMovementJob : IJobEntity
{
    public float deltaTime;
    public void Execute(ref LocalTransform localTransform, in UnitMovement unitMovement, ref PhysicsVelocity physicsVelocity)
    {
        var moveDirection = unitMovement.TargetPosition - localTransform.Position;

        if (math.lengthsq(moveDirection) < 2f)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;       
        }
        
        moveDirection = math.lengthsq(moveDirection) > 0f ? math.normalize(moveDirection) : float3.zero;
        localTransform.Rotation = 
            math.slerp(localTransform.Rotation, quaternion.LookRotationSafe(moveDirection, math.up()), deltaTime * unitMovement.RotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMovement.MoveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}