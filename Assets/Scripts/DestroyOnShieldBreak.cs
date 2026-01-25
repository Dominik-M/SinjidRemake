using UnityEngine;

public class DestroyOnShieldBreak : MonoBehaviour
{

    public void OnShieldBreak()
    {
        Destroy(gameObject);
    }
}
