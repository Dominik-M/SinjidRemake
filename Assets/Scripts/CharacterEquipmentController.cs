using UnityEngine;

public class CharacterEquipmentController : MonoBehaviour
{
    public Character mChar;
    public SpriteRenderer weaponSR, shieldSR, suitSR, headgearSR;

    public Character MChar
    {
        get => mChar; set
        {
            mChar = value;
            UpdateSprites();
        }
    }

    void Start()
    {
        UpdateSprites();
    }

    public void UpdateSprites()
    {
        if (!MChar)
            return;

        if (weaponSR && MChar.weapon)
            weaponSR.sprite = MChar.weapon.battleSprite;
        if (shieldSR && MChar.shield)
            shieldSR.sprite = MChar.shield.battleSprite;
        if (suitSR && MChar.suit)
            suitSR.sprite = MChar.suit.battleSprite;
        if (headgearSR && MChar.headgear)
            headgearSR.sprite = MChar.headgear.battleSprite;
    }
}
