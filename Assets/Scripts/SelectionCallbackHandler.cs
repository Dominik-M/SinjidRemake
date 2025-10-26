using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class SelectionCallbackHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int myId;
    public event Action<int> OnSelectItem;

    // Mouse hover (or touch on mobile)
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse entered: {gameObject.name}");
        OnSelectItem?.Invoke(myId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Mouse exited: {gameObject.name}");
        OnSelectItem?.Invoke(-1);
    }

    // Keyboard / Gamepad navigation
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"Button selected via navigation: {gameObject.name}");
        OnSelectItem?.Invoke(myId);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"Button deselected: {gameObject.name}");
        OnSelectItem?.Invoke(-1);
    }
}
