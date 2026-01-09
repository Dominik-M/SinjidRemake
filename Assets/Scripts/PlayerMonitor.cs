using UnityEngine;
using UnityEngine.UI;

public class PlayerMonitor : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private Text charname;
    [SerializeField] private Text classname;
    [SerializeField] private Image playericon;
    [SerializeField] private Text gold;
    [SerializeField] private Text level;
    [SerializeField] private Text life;
    [SerializeField] private Text maxlife;
    [SerializeField] private Slider lifebar;
    [SerializeField] private AnimatedImage lifebarAnimation;
    [SerializeField] private Text mana;
    [SerializeField] private Text maxmana;
    [SerializeField] private Slider manabar;
    [SerializeField] private AnimatedImage manabarAnimation;
    [SerializeField] private Text eng;
    [SerializeField] private Text maxeng;
    [SerializeField] private Slider engbar;
    [SerializeField] private AnimatedImage engbarAnimation;
    [SerializeField] private Text exp;
    [SerializeField] private Text nextexp;
    [SerializeField] private Slider expbar;
    [SerializeField] private AnimatedImage expbarAnimation;
    [SerializeField] private Text lifepotions;
    [SerializeField] private Text manapotions;
    [SerializeField] private AnimatedImage lifepotionsAnimation;
    [SerializeField] private AnimatedImage manapotionsAnimation;

    private int prevLife = 0;
    private int prevMana = 0;
    private int prevEng = 0;
    private int prevExp = 0;
    private int prevLifepotion = 0;
    private int prevManapotions = 0;

    void Start()
    {
        prevLife = (int)GameController.Life;
        prevMana = (int)GameController.Mana;
        prevEng = (int)GameController.Eng;
        prevExp = (int)GameController.Exp;
        prevLifepotion = GameController.LifePotions;
        prevManapotions = GameController.ManaPotions;
    }

    void FixedUpdate()
    {
        if (charname)
            charname.text = GameController.PlayerChar.displayName;
        if (classname)
            classname.text = GameController.MyClass.ToString();
        if (playericon)
            playericon.sprite = GameController.PlayerChar.icon;
        if (gold)
            gold.text = "Gold: " + GameController.Gold;
        if (level)
            level.text = "Level " + GameController.Level;

        if (life)
            life.text = GameController.Life.ToString("F0");
        if (maxlife)
            maxlife.text = GameController.MaxLife.ToString("F0");
        if (lifebar)
            lifebar.value = GameController.Life / GameController.MaxLife;
        if (lifebarAnimation != null && prevLife < (int)GameController.Life)
            lifebarAnimation.Restart();
        prevLife = (int)GameController.Life;

        if (mana)
            mana.text = GameController.Mana.ToString("F0");
        if (maxmana)
            maxmana.text = GameController.MaxMana.ToString("F0");
        if (manabar)
            manabar.value = GameController.Mana / GameController.MaxMana;
        if (manabarAnimation != null && prevMana < (int)GameController.Mana)
            manabarAnimation.Restart();
        prevMana = (int)GameController.Mana;

        if (eng)
            eng.text = GameController.Eng.ToString("F0");
        if (maxeng)
            maxeng.text = GameController.MaxEng.ToString("F0");
        if (engbar)
            engbar.value = GameController.Eng / GameController.MaxEng;
        if (engbarAnimation != null && prevEng < (int)GameController.Eng)
            engbarAnimation.Restart();
        prevEng = (int)GameController.Eng;

        if (exp)
            exp.text = GameController.Exp.ToString("F0");
        if (nextexp)
            nextexp.text = GameController.ExpNext.ToString("F0");
        if (expbar)
            expbar.value = GameController.Exp / GameController.ExpNext;
        if (expbarAnimation != null && prevExp < (int)GameController.Exp)
            expbarAnimation.Restart();
        prevExp = (int)GameController.Exp;

        if (lifepotions)
            lifepotions.text = "x " + GameController.LifePotions;
        if (lifepotionsAnimation != null && prevLifepotion != GameController.LifePotions)
            lifepotionsAnimation.Restart();
        prevLifepotion = GameController.LifePotions;

        if (manapotions)
            manapotions.text = "x " + GameController.ManaPotions;
        if (manapotionsAnimation != null && prevManapotions != GameController.ManaPotions)
            manapotionsAnimation.Restart();
        prevManapotions = GameController.ManaPotions;
    }
}
