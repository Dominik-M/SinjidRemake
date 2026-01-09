using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    public void OnEnable()
    {
        GetComponent<Selectable>().Select();
    }
}
