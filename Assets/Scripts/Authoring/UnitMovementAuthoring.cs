using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMovementAuthoring : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    
    [SerializeField]
    private float rotationSpeed;

    public class MoveSpeedBaker : Baker<UnitMovementAuthoring>
    {
        public override void Bake(UnitMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMovement
            {
                MoveSpeed = authoring.moveSpeed,
                RotationSpeed = authoring.rotationSpeed
            });
        }
    }
}

public struct UnitMovement : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public float3 TargetPosition;
}