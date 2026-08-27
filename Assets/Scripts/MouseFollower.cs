using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseFollower : MonoBehaviour
{
    private Vector3 lastMousePos = Vector3.zero;

    void FixedUpdate()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected)
            transform.position = selected.transform.position;
        Vector2 mousePos = Pointer.current.position.value;
        if (!mousePos.Equals(lastMousePos))
        {
            transform.position = mousePos;
            lastMousePos = mousePos;
        }
    }
}
