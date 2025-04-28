using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    class UnitAuthoringBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit());
        }
    }
}

public struct Unit : IComponentData
{
}