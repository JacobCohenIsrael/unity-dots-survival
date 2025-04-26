using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetMouseWorldPosition()
    {
        var mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, Vector3.zero);
        return plane.Raycast(mouseCameraRay, out var distance) ? mouseCameraRay.GetPoint(distance) : Vector3.zero;
    }
}
