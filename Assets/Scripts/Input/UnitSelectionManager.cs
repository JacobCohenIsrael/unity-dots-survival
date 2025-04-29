using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    
    [SerializeField] private float multiSelectThreshold = 40f;

    public event Action OnSelectionAreaStart;
    public event Action OnSelectionAreaEnd;
    
    private Vector2 selectionStartMousePosition;
    

    public static UnitSelectionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke();       
        }

        if (Input.GetMouseButtonUp(0))
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
           
            // Deselect all selected
            var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithAll<Selection>().Build(entityManager);
            var entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            for (var index = 0; index < entityArray.Length; index++)
            {
                entityManager.SetComponentEnabled<Selection>(entityArray[index], false);
            }
            
            var selectionAreaRect = GetSelectionAreaRect();

            if (selectionAreaRect.width + selectionAreaRect.height > multiSelectThreshold)
            {
                // Select all units within the selection area
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selection>().Build(entityManager);
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                var localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (var index = 0; index < localTransformArray.Length; index++)
                {
                    var unitLocalTransform = localTransformArray[index];
                    var unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        entityManager.SetComponentEnabled<Selection>(entityArray[index], true);
                    }
                }
            }
            else
            {
                // Single Select
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                var physicsWorld = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                var collisionWorld = physicsWorld.CollisionWorld;
                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                const uint unitLayer = 1 << 6;
                var raycastInput = new RaycastInput()
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(1000f),
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = ~0u,
                        CollidesWith = unitLayer,
                        GroupIndex = 0
                    },
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
                {
                    if (entityManager.HasComponent<Unit>(hit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selection>(hit.Entity, true);
                    }
                }
            }
            OnSelectionAreaEnd?.Invoke();  

        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = InputManager.Instance.GetMouseWorldPosition();
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMovement, Selection>().Build(entityManager);
            var unitMovementArray = entityQuery.ToComponentDataArray<UnitMovement>(Allocator.Temp);
            for (var index = 0; index < unitMovementArray.Length; index++)
            {
                var unitMovement = unitMovementArray[index];
                unitMovement.TargetPosition = mousePosition;
                unitMovementArray[index] = unitMovement;
            }
            entityQuery.CopyFromComponentDataArray(unitMovementArray);
        }
    }

    public Rect GetSelectionAreaRect()
    {
        var selectionEndMousePosition = Input.mousePosition;
        
        var lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));
        
        var upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));
        
        return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x, upperRightCorner.y - lowerLeftCorner.y);
    }
}
