using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = InputManager.Instance.GetMouseWorldPosition();
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMovement>().Build(entityManager);
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
}
