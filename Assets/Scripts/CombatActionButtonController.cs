using UnityEngine;
using UnityEngine.UI;

public class CombatActionButtonController : MonoBehaviour
{
    public Skill skill;

    [Header("UI References")]
    [SerializeField] private CombatController combat;
    [SerializeField] private GameObject locked, skilldetails;
    [SerializeField] private Image icon;
    [SerializeField] private Text skillname, manacost;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        GetComponent<SelectionCallbackHandler>().OnSelectItem += OnSelected;
        locked.SetActive(skill.currentlevel <= 0);
        icon.sprite = skill.icon;
    }

    public void OnSelected(int id)
    {
        if(id < 0 || skill.currentlevel <= 0)
        {
            skilldetails.SetActive(false);
        }
        else
        {
            skilldetails.SetActive(true);
            skillname.text = Localization.GetText(skill.nameTextId);
            manacost.text = skill.manacost.ToString();
        }
    }

    public void OnClick()
    {
        combat.OnSkillButtonClicked(skill);
    }
}
