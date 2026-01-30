
using UnityEngine;
using UnityEngine.UI;

public class PotionScreenController : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private Text life;
    [SerializeField] private Text maxlife;
    [SerializeField] private Slider lifebar;
    [SerializeField] private Text mana;
    [SerializeField] private Text maxmana;
    [SerializeField] private Slider manabar;
    [SerializeField] private Text gold;
    [SerializeField] private Text currentLifepotions, currentManapotions;

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateValues();
    }

    void UpdateValues()
    {
        // Life
        float n = GameController.Life;
        float mn = GameController.MaxLife;
        life.text = n.ToString("F0");
        maxlife.text = mn.ToString("F0");
        lifebar.value = n / mn;
        // Mana
        n = GameController.Mana;
        mn = GameController.MaxMana;
        mana.text = n.ToString("F0");
        maxmana.text = mn.ToString("F0");
        manabar.value = n / mn;
        // Gold
        gold.text = Localization.GetText(1296) + GameController.Gold;
        // Potions
        currentLifepotions.text = GameController.LifePotions.ToString();
        currentManapotions.text = GameController.ManaPotions.ToString();
    }

    public void OnLifepotion()
    {
        if (GameController.Gold >= 25)
        {
            GameController.Gold -= 25;
            GameController.LifePotions++;
            UpdateValues();
        }
    }
    public void OnManapotion()
    {
        if (GameController.Gold >= 15)
        {
            GameController.Gold -= 15;
            GameController.ManaPotions++;
            UpdateValues();
        }
    }
}

