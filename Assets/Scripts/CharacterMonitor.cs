using UnityEngine;
using UnityEngine.UI;

public class CharacterMonitor : MonoBehaviour
{
    private Character mChar;

    [Header("UI References")]
    [SerializeField] private Text charname;
    [SerializeField] private Image charicon;
    [SerializeField] private Text life;
    [SerializeField] private Slider lifebar;
    [SerializeField] private Text shield;
    [SerializeField] private Slider shieldbar;

    public Character MChar
    {
        get => mChar; set
        {
            mChar = value;
            gameObject.SetActive(mChar != null);
            if(mChar!= null)
            {
                charname.text = Localization.GetText(mChar.displayNameTextId);
                charicon.sprite = mChar.icon;
            }
        }
    }

    void Start()
    {
        if (MChar == null)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        float currentlife = MChar.life;
        float maxlife = MChar.maxlife;
        float currentshield = MChar.currentShieldHp;
        float maxshield = (MChar.shield != null) ? MChar.shield.shieldHP : 0;
        life.text = currentlife.ToString("F0");
        lifebar.value = (maxlife == 0) ? 0 : (currentlife / maxlife);
        shield.text = currentshield.ToString("F0");
        shieldbar.value = (maxshield == 0) ? 0 : (currentshield / maxshield);
    }
}
