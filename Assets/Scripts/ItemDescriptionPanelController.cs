using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject physDmg;
    [SerializeField] private GameObject physDef;
    [SerializeField] private GameObject magicDmg;
    [SerializeField] private GameObject magicDef;
    [SerializeField] private GameObject shieldDmg;
    [SerializeField] private GameObject shieldHp;
    [SerializeField] private GameObject bonusMana;
    [SerializeField] private GameObject bonusSpeed;

    [Header("Texts")]
    [SerializeField] private Text itemname;
    [SerializeField] private Text itemtype;
    [SerializeField] private Text descriptiontext;
    [SerializeField] private Text reqStrValue;
    [SerializeField] private Text physDmgValue;
    [SerializeField] private Text physDefValue;
    [SerializeField] private Text magicDmgValue;
    [SerializeField] private Text magicDefValue;
    [SerializeField] private Text shieldDmgValue;
    [SerializeField] private Text shieldHpValue;
    [SerializeField] private Text bonusManaValue;
    [SerializeField] private Text bonusSpeedValue;

    private Item item;

    public Item DisplayedItem
    {
        get => item; set
        {
            item = value;
            UpdateLabels();
        }
    }

    public static bool IsEquipment(Item.Type type)
    {
        return type == Item.Type.Weapon || type == Item.Type.Shield || type == Item.Type.Suit || type == Item.Type.Head_Gear;
    }

    public void UpdateLabels()
    {
        if(item == null)
        {
            panel.SetActive(false);
            return;
        }
        panel.SetActive(true);
        // Common values
        itemname.text = Localization.GetText(item.displaynameTextId);
        itemtype.text = item.GetTypeText();
        reqStrValue.text = item.strengthRequired.ToString();
        // Equipment stats
        physDmg.SetActive(item.physDmg != 0);
        physDmgValue.text = item.physDmg.ToString();
        physDef.SetActive(item.physDef != 0);
        physDefValue.text = item.physDef.ToString();
        magicDmg.SetActive(item.magicDmg != 0);
        magicDmgValue.text = item.magicDmg.ToString();
        magicDef.SetActive(item.magicDef != 0);
        magicDefValue.text = item.magicDef.ToString();
        shieldDmg.SetActive(item.shieldDmg != 0);
        shieldDmgValue.text = item.shieldDmg.ToString();
        shieldHp.SetActive(item.shieldHP != 0);
        shieldHpValue.text = item.shieldHP.ToString();
        bonusMana.SetActive(item.bonusMana != 0);
        bonusManaValue.text = item.bonusMana.ToString();
        bonusSpeed.SetActive(item.bonusSpeed != 0);
        bonusSpeedValue.text = item.bonusSpeed.ToString();

        if (IsEquipment(item.type))
        {
            description.SetActive(false);
        }
        else
        {
            // special item without stats, only show description
            description.SetActive(true);
            descriptiontext.text = Localization.GetText(item.descriptionTextId);
        }
    }
}
