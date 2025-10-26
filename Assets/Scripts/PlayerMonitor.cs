using UnityEngine;
using UnityEngine.UI;

public class PlayerMonitor : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private Text gold;
    [SerializeField] private Text level;
    [SerializeField] private Text life;
    [SerializeField] private Text maxlife;
    [SerializeField] private Slider lifebar;
    [SerializeField] private Text mana;
    [SerializeField] private Text maxmana;
    [SerializeField] private Slider manabar;
    [SerializeField] private Text eng;
    [SerializeField] private Text maxeng;
    [SerializeField] private Slider engbar;

    void Update()
    {
        if (gold)
            gold.text = "Gold: "+GameController.Gold;
        if (level)
            level.text = "Level " + GameController.Level;
        if (life)
            life.text = GameController.Life.ToString("F0");
        if (maxlife)
            maxlife.text = GameController.MaxLife.ToString("F0");
        if (lifebar)
            lifebar.value = GameController.Life / GameController.MaxLife;

        if (mana)
            mana.text = GameController.Mana.ToString("F0");
        if (maxmana)
            maxmana.text = GameController.MaxMana.ToString("F0");
        if (manabar)
            manabar.value = GameController.Mana / GameController.MaxMana;

        if (eng)
            eng.text = GameController.Eng.ToString("F0");
        if (maxeng)
            maxeng.text = GameController.MaxEng.ToString("F0");
        if (engbar)
            engbar.value = GameController.Eng / GameController.MaxEng;
    }
}
