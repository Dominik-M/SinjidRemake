using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public enum Type
    {
        Weapon, Shield, Head_Gear, Suit, Herb, Drink, Relic
    }
    public Type type;
    public Sprite icon, battleSprite;
    public string displayname, description;
    public int displaynameTextId, descriptionTextId;
    public int value; // negative values mean not sellable
    public int strengthRequired;
    public int physDmg, magicDmg, shieldDmg;
    public int physDef, magicDef, shieldHP;
    public int bonusMana, bonusSpeed;
    public bool dropable;

    public string GetTypeText()
    {
        switch (type)
        {
            case Type.Weapon:
                return Localization.GetText(1004);
            case Type.Shield:
                return Localization.GetText(1007);
            case Type.Head_Gear:
                return Localization.GetText(1005);
            case Type.Suit:
                return Localization.GetText(1006);
            case Type.Herb:
                return Localization.GetText(1016);
            case Type.Drink:
                return Localization.GetText(1017);
            case Type.Relic:
                return Localization.GetText(1018);
            default:
                return "undefined";
        }
    }
}
