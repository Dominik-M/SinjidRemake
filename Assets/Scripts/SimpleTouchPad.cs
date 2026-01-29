using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SimpleTouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public float smoothing;
    public Vector2 maxDrag = new Vector2(200, 200);
    public RectTransform knobTransform;
    public RectTransform knobBGTransform;

    private Vector2 origin;
    private Vector2 direction;
    private bool touched;
    private int pointerID;

    public bool Touched
    {
        get => touched; set
        {
            touched = value;
            if (knobTransform)
            {
                knobTransform.gameObject.SetActive(touched);
            }
        }
    }

    void Awake()
    {
        direction = Vector2.zero;
        Touched = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!Touched)
        {
            Touched = true;
            pointerID = data.pointerId;
            origin = data.position;
            if (knobTransform)
                knobTransform.position = origin;
            if (knobBGTransform)
                knobBGTransform.position = origin;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            Vector2 currentPosition = data.position;
            Vector2 directionRaw = currentPosition - origin;
            direction = new Vector2(
                Mathf.Clamp(directionRaw.x, -1 * maxDrag.x, maxDrag.x),
                Mathf.Clamp(directionRaw.y, -1 * maxDrag.y, maxDrag.y)
                );
            if (knobTransform)
                knobTransform.position = origin + direction;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            direction = Vector3.zero;
            Touched = false;
            if (knobTransform)
                knobTransform.localPosition = direction;
        }
    }

    public Vector2 GetDirection()
    {
        //Debug.Log("GetDirection(): " + direction);
        return direction.normalized;
    }
}