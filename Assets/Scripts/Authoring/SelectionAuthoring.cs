using Unity.Entities;
using UnityEngine;

public class SelectionAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    [SerializeField] private float showScale;
    class SelectionAuthoringBaker : Baker<SelectionAuthoring>
    {
        public override void Bake(SelectionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selection
            {
                VisualEntity = GetEntity(authoring.visual, TransformUsageFlags.Dynamic),
                ShowScale = authoring.showScale
            });
            SetComponentEnabled<Selection>(entity, false);
        }
    }
}

public struct Selection : IComponentData, IEnableableComponent
{
    public Entity VisualEntity;
    public float ShowScale;
}