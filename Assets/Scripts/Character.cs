using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public GameObject combatprefab;
    public float life, maxlife;
    public float mana, maxmana;
    public float eng, maxeng;
    public float exp, expNext;
    public int gold, level, lifePotions, manaPotions;
    public int strength, dex, magic;
    public int currentShieldHp;
    public Item weapon, shield, suit, headgear;

    public bool IsPlayerChar()
    {
        return name.Equals("Player");
    }

    public int GetPhysDmg()
    {
        int n = 1;
        if (weapon)
            n += weapon.physDmg;
        if (suit)
            n += suit.physDmg;
        if (headgear)
            n += headgear.physDmg;
        float scaling = 1 + 0.2f * strength + 0.2f * dex;
        n = (int)(n * scaling);
        if (IsPlayerChar())
        {
            n += GameController.FindSkillByName("Inner Strength").GetCurrentLevelValue();
        }
        return n;
    }

    public int GetMagicDmg()
    {
        int n = 0;
        if (weapon)
            n += weapon.magicDmg;
        if (suit)
            n += suit.magicDmg;
        if (headgear)
            n += headgear.magicDmg;
        float scaling = 1 + 0.5f * magic;
        n = (int)(n * scaling);
        if (IsPlayerChar())
        {
            n += GameController.FindSkillByName("Energy Field").GetCurrentLevelValue();
        }
        return n;
    }

    public float GetPhysDefPercent()
    {
        float reduction = 0f;
        if (weapon) reduction = 1 - (1 - reduction) * (1 - weapon.physDef / 100f);
        if (suit) reduction = 1 - (1 - reduction) * (1 - suit.physDef / 100f);
        if (headgear) reduction = 1 - (1 - reduction) * (1 - headgear.physDef / 100f);
        return reduction;
    }

    public float GetMagicDefPercent()
    {
        float reduction = 0f;
        if (weapon) reduction = 1 - (1 - reduction) * (1 - weapon.magicDef / 100f);
        if (suit) reduction = 1 - (1 - reduction) * (1 - suit.magicDef / 100f);
        if (headgear) reduction = 1 - (1 - reduction) * (1 - headgear.magicDef / 100f);
        return reduction;
    }

    public float GetShieldPhysDefPercent()
    {
        float reduction = 0f;
        if (shield) reduction = 1 - (1 - reduction) * (1 - shield.physDef / 100f);
        return reduction;
    }

    public float GetShieldMagicDefPercent()
    {
        float reduction = 0f;
        if (shield) reduction = 1 - (1 - reduction) * (1 - shield.magicDef / 100f);
        return reduction;
    }

    public int GetSpeed()
    {
        int spd = dex;
        if (weapon)
            spd += weapon.bonusSpeed;
        if (suit)
            spd += suit.bonusSpeed;
        if (headgear)
            spd += headgear.bonusSpeed;
        if (shield)
            spd += shield.bonusSpeed;
        if (IsPlayerChar())
        {
            spd += GameController.FindSkillByName("Shadow Blend").GetCurrentLevelValue();
        }
        return spd;
    }

    public float GetDodgeChance()
    {
        float percent;
        int spd = GetSpeed();

        if (spd < 20)
            percent = spd * 1.0f; // max 20%
        else if (spd < 40)
            percent = 10 + spd * 0.5f; // max 30%
        else if (spd < 100)
            percent = 18 + spd * 0.3f; // max 48%
        else if (spd < 198)
            percent = 28 + spd * 0.2f; // max 68%
        else
            percent = 68.0f;

        return percent / 100.0f;
    }
}
