using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    public Image icon, background;
    public Sprite defaultBg, defaultIcon;
    public Sprite bgWeapon, bgShield, bgHead_Gear, bgSuit, bgHerb, bgDrink, bgRelic;

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
        if(item != null)
        {
            icon.sprite = item.icon;
            background.sprite = GetItemTypeBackground(item.type);
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
}
