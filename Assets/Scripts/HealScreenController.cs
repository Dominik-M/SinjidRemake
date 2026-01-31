
using UnityEngine;
using UnityEngine.UI;
public class HealScreenController : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private Text life;
    [SerializeField] private Text maxlife;
    [SerializeField] private Slider lifebar;
    [SerializeField] private AnimatedImage lifebarImage;
    [SerializeField] private Text mana;
    [SerializeField] private Text maxmana;
    [SerializeField] private Slider manabar;
    [SerializeField] private AnimatedImage manabarImage;
    [SerializeField] private Text gold;

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
    }

    public void OnLifeHeal()
    {
        if (GameController.Gold >= 10)
        {
            GameController.Gold -= 10;
            GameController.Life += 40;
            UpdateValues();
            lifebarImage.Restart();
        }
    }
    public void OnManaHeal()
    {
        if (GameController.Gold >= 5)
        {
            GameController.Gold -= 5;
            GameController.Mana += 40;
            UpdateValues();
            manabarImage.Restart();
        }
    }

    public void OnHealAll()
    {
        if (GameController.Gold >= 20)
        {
            GameController.Gold -= 20;
            GameController.Life += 60;
            GameController.Mana += 60;
            UpdateValues();
            lifebarImage.Restart();
            manabarImage.Restart();
        }
    }

    public void OnRestoration()
    {
        if (GameController.Gold >= 30)
        {
            GameController.Gold -= 30;
            GameController.Life += 120;
            GameController.Mana += 120;
            UpdateValues();
            lifebarImage.Restart();
            manabarImage.Restart();
        }
    }
}
