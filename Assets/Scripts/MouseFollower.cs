using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        transform.position = mousePos;
    }
}
