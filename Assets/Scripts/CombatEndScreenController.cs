using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatEndScreenController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject defeatButtons;
    [SerializeField] private GameObject victoryButtons;
    [SerializeField] private GameObject itemdropMessage;
    [SerializeField] private GameObject levelUpScreen;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject skilltreeScreen;
    [SerializeField] private Text title;
    [SerializeField] private Text goldreceived, totalgold;
    [SerializeField] private Text expgained, currentexp, nextexp, leveltext, leveluptext;
    [SerializeField] private Text lifegained, managained, enggained, strengthgained, dexgained, magicgained;
    [SerializeField] private Text strength, dex, magic, maxlife, maxmana, pointsRemain;
    [SerializeField] private Slider expBar;
    [SerializeField] private AnimatedImage expBarAnimation;

    private int levelsGained = 0;

    public void ShowEndscreen(bool defeat, int goldgained, int xpgained, Item drop)
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        defeatButtons.SetActive(defeat);
        victoryButtons.SetActive(!defeat);
        if (!defeat)
        {
            title.text = Localization.GetText(2153);
            itemdropMessage.SetActive(drop != null && drop.dropable);
            StartCoroutine(EarnGoldAndXP(goldgained, xpgained));
            if (drop != null && drop.dropable)
            {
                for (int i = 0; i < GameController.GetTotalInventorySpace(); i++)
                {
                    if (GameController.GetInventoryItem(i) == null)
                    {
                        GameController.SetInventoryItem(i, drop);
                        break;
                    }
                }
            }
        }
        else
        {
            title.text = Localization.GetText(2251);
        }
    }

    public void OnContinuePressed()
    {
        if (levelsGained > 0)
        {
            victoryButtons.SetActive(false);
            levelUpScreen.SetActive(true);
            UpdateAttributesTexts();
        }
        else
        {
            CombatController.Instance.OnEndScreenExit();
        }
    }

    public void OnLevelUpContinue()
    {
        levelUpScreen.SetActive(false);
        skilltreeScreen.SetActive(true);
    }

    public void UpdateAttributesTexts()
    {
        continueButton.SetActive(levelsGained <= 0);
        pointsRemain.text = "" + levelsGained;
        strength.text = "" + GameController.Strength;
        dex.text = "" + GameController.Dex;
        magic.text = "" + GameController.Magic;
        maxlife.text = "" + GameController.MaxLife;
        maxmana.text = "" + GameController.MaxMana;
    }

    public void OnAttributeButtonPressed(int idx)
    {
        if (levelsGained > 0)
        {
            levelsGained--;
            switch (idx)
            {
                case 0:
                    // strength
                    GameController.Strength++;
                    break;
                case 1:
                    // dex
                    GameController.Dex++;
                    break;
                case 2:
                    // magic
                    GameController.Magic ++;
                    break;
                case 3:
                    // life
                    GameController.MaxLife+=10;
                    GameController.Life = GameController.MaxLife;
                    break;
                case 4:
                    // mana
                    GameController.MaxMana = GameController.PlayerChar.maxmana + 10;
                    GameController.Mana = GameController.MaxMana;
                    break;
            }
        }
        UpdateAttributesTexts();
    }

    private IEnumerator EarnGoldAndXP(int gold, int xp)
    {
        int prevMaxhp = (int)GameController.MaxLife;
        int prevMaxmana = (int)GameController.MaxMana;
        int prevMaxeng = (int)GameController.MaxEng;
        int prevStr = GameController.Strength;
        int prevDex = GameController.Dex;
        int prevMagic = GameController.Magic;
        int currentgold = GameController.Gold;
        float currentxp = GameController.Exp;
        float nextxp = GameController.ExpNext;

        // initial values
        expgained.text = Localization.GetText(2157) + xp;
        currentexp.text = currentxp.ToString("F0") + " / ";
        nextexp.text = nextxp.ToString("F0");
        expBar.value = currentxp / nextxp;
        goldreceived.text = gold.ToString();
        totalgold.text = currentgold.ToString();

        // First add the gold and xp to not loose if the animation is skipped
        int currentLevel = GameController.Level;
        GameController.Exp += xp;
        GameController.Gold += gold;
        levelsGained = (GameController.Level - currentLevel) * GameController.StatPointsPerLevel;

        // Animate gold gained
        while (gold > 0)
        {
            gold--;
            currentgold++;
            yield return new WaitForSeconds(0.01f);

            goldreceived.text = gold.ToString();
            totalgold.text = currentgold.ToString();
        }

        // Animate the xp gain
        while (xp > 0)
        {
            xp--;
            currentxp++;
            yield return null;

            expgained.text = Localization.GetText(2157) + xp;
            currentexp.text = currentxp.ToString("F0") + " / ";
            nextexp.text = nextxp.ToString("F0");
            expBar.value = currentxp / nextxp;
            if (currentxp >= nextxp)
            {
                // Level up
                expgained.gameObject.SetActive(false);
                currentexp.gameObject.SetActive(false);
                nextexp.gameObject.SetActive(false);
                leveltext.text = "Level: " + GameController.Level;
                leveltext.gameObject.SetActive(true);
                leveluptext.gameObject.SetActive(true);
                expBarAnimation.Restart();
                int lifeGain = (int)GameController.MaxLife - prevMaxhp;
                int manaGain = (int)GameController.MaxMana - prevMaxmana;
                int engGain = (int)GameController.MaxEng - prevMaxeng;
                int strGain = (int)GameController.Strength - prevStr;
                int dexGain = (int)GameController.Dex - prevDex;
                int magGain = (int)GameController.Magic - prevMagic;
                if (lifeGain > 0)
                {
                    lifegained.gameObject.SetActive(true);
                    lifegained.text = "Max Life +" + lifeGain;
                }
                if (manaGain > 0)
                {
                    managained.gameObject.SetActive(true);
                    managained.text = "Max Mana +" + manaGain;
                }
                if (engGain > 0)
                {
                    enggained.gameObject.SetActive(true);
                    enggained.text = "Max Energy +" + engGain;
                }
                if (strGain > 0)
                {
                    strengthgained.gameObject.SetActive(true);
                    strengthgained.text = "Strength +" + strGain;
                }
                if (dexGain > 0)
                {
                    dexgained.gameObject.SetActive(true);
                    dexgained.text = "Dexterity +" + dexGain;
                }
                if (magGain > 0)
                {
                    magicgained.gameObject.SetActive(true);
                    magicgained.text = "Magic +" + magGain;
                }
                break; // only animate one bar
            }
        }
    }

}
