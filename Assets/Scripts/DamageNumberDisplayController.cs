using UnityEngine;

public class DamageNumberDisplayController : MonoBehaviour
{
    public GameObject prefab;

    public void Show(Vector3 position, int value, Color textcolor)
    {
        GameObject instance = Instantiate(prefab, transform);
        instance.transform.localPosition = position;
        PopupController popup = instance.GetComponent<PopupController>();
        popup.Value = value;
        popup.Textcolor = textcolor;
    }

}
