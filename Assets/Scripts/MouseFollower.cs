using UnityEngine;
using UnityEngine.EventSystems;

public class MouseFollower : MonoBehaviour
{
    private Vector3 lastMousePos = Vector3.zero;

    void FixedUpdate()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected)
            transform.position = selected.transform.position;
        Vector3 mousePos = Input.mousePosition;
        if (!mousePos.Equals(lastMousePos))
        {
            transform.position = mousePos;
            lastMousePos = mousePos;
        }
    }
}
