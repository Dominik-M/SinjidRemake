using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ItemSlotController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon, background;
    public Sprite defaultBg, defaultIcon;
    public Sprite bgWeapon, bgShield, bgHead_Gear, bgSuit, bgHerb, bgDrink, bgRelic;
    public Text costText;

    public event Action<Item> OnSelectItem;

    private Item item;

    public Item Item
    {
        get => item; set
        {
            item = value;
            UpdateImages();
        }
    }

    void OnEnable()
    {
        UpdateImages();
    }

    void UpdateImages()
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            background.sprite = GetItemTypeBackground(item.type);
            if (costText != null)
                costText.text = item.value.ToString();
        }
        else
        {
            icon.sprite = defaultIcon;
            background.sprite = defaultBg;
        }
    }

    public Sprite GetItemTypeBackground(Item.Type t)
    {
        switch (t)
        {
            case Item.Type.Weapon:
                return bgWeapon;
            case Item.Type.Shield:
                return bgShield;
            case Item.Type.Head_Gear:
                return bgHead_Gear;
            case Item.Type.Suit:
                return bgSuit;
            case Item.Type.Herb:
                return bgHerb;
            case Item.Type.Drink:
                return bgDrink;
            case Item.Type.Relic:
            default:
                return bgRelic;
        }
    }    
    
    // Mouse hover (or touch on mobile)
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse entered: {gameObject.name}");
        OnSelectItem?.Invoke(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Mouse exited: {gameObject.name}");
        OnSelectItem?.Invoke(null);
    }

    // Keyboard / Gamepad navigation
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"Button selected via navigation: {gameObject.name}");
        OnSelectItem?.Invoke(item);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log($"Button deselected: {gameObject.name}");
        OnSelectItem?.Invoke(null);
    }
}
