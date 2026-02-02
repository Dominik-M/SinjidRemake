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
}
