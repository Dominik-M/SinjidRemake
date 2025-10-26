using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        transform.position = mousePos;
    }
}
