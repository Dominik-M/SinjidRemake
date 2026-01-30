using UnityEngine;
using UnityEngine.UI;

public class RestScreenController : DefaultMenuButtonHandler
{
    public Image playericon;
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
    public Text restsremainingNumber;
    public GameObject savepopup;

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
        playericon.sprite = GameController.PlayerChar.icon;
        level.text = "Level: "+GameController.Level;
        mClass.text = GameController.MyClassname;
        gold.text = Localization.GetText(1296) + GameController.Gold.ToString();
        lifepotions.text = "x " + GameController.LifePotions;
        manapotions.text = "x " + GameController.ManaPotions;
        physDmg.text = Localization.GetText(1334) + GameController.GetPhysDmg();
        physDef.text = Localization.GetText(1336) + GameController.GetPhysDef();
        magicDmg.text = Localization.GetText(1335) + GameController.GetMagicDmg();
        magicDef.text = Localization.GetText(1337) + GameController.GetMagicDef();
        strength.text = GameController.Strength.ToString();
        dex.text = GameController.Dex.ToString();
        magic.text = GameController.Magic.ToString();
        restsremainingNumber.text = GameController.RestsRemaining.ToString();
    }

    public void OnRest()
    {
        Debug.Log("OnRest()");
        if (GameController.RestsRemaining > 0)
        {
            GameController.RestsRemaining--;
            GameController.Life = GameController.MaxLife;
            GameController.Mana = GameController.MaxMana;
            GameController.Eng = GameController.MaxEng;
            UpdateValues();
            lifebarImage.Restart();
            manabarImage.Restart();
            engbarImage.Restart();
        }
    }
    public void OnSave()
    {
        Debug.Log("OnSave()");
        GameController.SaveAllPrefs();
        Instantiate(savepopup, transform);
    }
}
