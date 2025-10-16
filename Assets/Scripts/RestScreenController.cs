using UnityEngine;
using UnityEngine.UI;

public class RestScreenController : DefaultMenuButtonHandler
{
    public Text life, maxlife;
    public Slider lifebar;
    public AnimatedImage lifebarImage;
    public Text mana, maxmana;
    public Slider manabar;
    public AnimatedImage manabarImage;
    public Text eng, maxeng;
    public Slider engbar;
    public AnimatedImage engbarImage;
    public Text mClass, level, gold, lifepotions, manapotions;
    public Text physDmg, magicDmg, physDef, magicDef;
    public Text strength, dex, magic;

    void Awake()
    {
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
        // Eng
        n = GameController.Eng;
        mn = GameController.MaxEng;
        eng.text = n.ToString("F0");
        maxeng.text = mn.ToString("F0");
        engbar.value = n / mn;
        // Stats
        level.text = "Level: "+GameController.Level;
        mClass.text = GameController.MyClass.ToString();
        gold.text = "Gold: "+GameController.Gold.ToString();
        lifepotions.text = "x " + GameController.LifePotions;
        manapotions.text = "x " + GameController.ManaPotions;
        physDmg.text = "Physical   Damage:    " + GameController.PhysDmg;
        physDef.text = "Physical   Defence:    " + GameController.PhysDef;
        magicDmg.text = "Magical   Damage:    " + GameController.MagicDmg;
        magicDef.text = "Magical   Defence:    " + GameController.MagicDef;
        strength.text = GameController.Strength.ToString();
        dex.text = GameController.Dex.ToString();
        magic.text = GameController.Magic.ToString();
    }

    public void OnRest()
    {
        GameController.Life = GameController.MaxLife;
        GameController.Mana = GameController.MaxMana;
        GameController.Eng = GameController.MaxEng;
        UpdateValues();
        lifebarImage.Restart();
        manabarImage.Restart();
        engbarImage.Restart();
    }
}
