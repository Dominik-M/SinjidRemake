using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatEndScreenController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject defeatButtons;
    [SerializeField] private GameObject victoryButtons;
    [SerializeField] private GameObject itemdropMessage;
    [SerializeField] private Text title;
    [SerializeField] private Text goldreceived, totalgold;
    [SerializeField] private Text expgained, currentexp, nextexp, leveltext, leveluptext;
    [SerializeField] private Text lifegained, managained, enggained, strengthgained, dexgained, magicgained;
    [SerializeField] private Slider expBar;
    [SerializeField] private AnimatedImage expBarAnimation;

    public void ShowEndscreen(bool defeat, int goldgained, int xpgained, Item drop)
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        defeatButtons.SetActive(defeat);
        victoryButtons.SetActive(!defeat);
        if (!defeat)
        {
            title.text = "VICTORY";
            itemdropMessage.SetActive(drop != null);
            StartCoroutine(EarnGoldAndXP(goldgained, xpgained));
            if (drop != null)
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
            title.text = "DEFEAT";
        }
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
        expgained.text = "Experience Gained: " + xp;
        currentexp.text = currentxp.ToString("F0") + " / ";
        nextexp.text = nextxp.ToString("F0");
        expBar.value = currentxp / nextxp;
        goldreceived.text = gold.ToString();
        totalgold.text = currentgold.ToString();

        // First add the gold and xp to not loose if the animation is skipped
        GameController.Exp += xp;
        GameController.Gold += gold;

        // Animate gold gained
        while (gold > 0)
        {
            gold--;
            currentgold++;
            yield return new WaitForSeconds(0.02f);

            goldreceived.text = gold.ToString();
            totalgold.text = currentgold.ToString();
        }

        // Animate the xp gain
        while (xp > 0)
        {
            xp--;
            currentxp++;
            yield return new WaitForSeconds(0.05f);

            expgained.text = "Experience Gained: " + xp;
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
