using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Mouse hover (or touch on mobile)
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse entered: {gameObject.name}");
        GetComponent<Selectable>().Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Mouse exited: {gameObject.name}");
    }
}
