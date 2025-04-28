using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
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
            
            // Select all units within selection area
            var selectionAreaRect = GetSelectionAreaRect();
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
