using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Skill skill;
    [Header("UI Elements")]
    [SerializeField] private Text level;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Text descriptionTitle;
    [SerializeField] private Text description;
    [SerializeField] private Text manacost;
    [SerializeField] private Text effectTitle;
    [SerializeField] private Text effectValue;
    [SerializeField] private Text effectValueNext;
    [SerializeField] private Image icon;

    public void Start()
    {
        GetComponent<SelectionCallbackHandler>().OnSelectItem += OnSelectionChanged;
        icon.sprite = skill.icon;
    }
    public void Update()
    {
        level.text = skill.currentlevel.ToString();
        lockedPanel.SetActive((skill.currentlevel == 0) && (!skill.CanUpgrade()));
    }

    public void UpdateDescription()
    {
        descriptionTitle.text = skill.displayname;
        description.text = skill.description;
        effectTitle.text = skill.effectTitle;
        effectValue.text = skill.GetCurrentLevelValueDescription();
        effectValueNext.text = skill.GetNextLevelValueDescription();
        if (skill.passive)
        {
            manacost.text = "Passive";
            manacost.color = Color.green;
            effectTitle.color = Color.green;
        }
        else
        {
            manacost.text = "Requires " + skill.manacost +" Mana";
            manacost.color = Color.cyan;
            effectTitle.color = Color.orange;
        }
    }

    public void OnClick()
    {
        if(skill.CanUpgrade() && GameController.Skillpoints > 0)
        {
            GameController.Skillpoints--;
            skill.currentlevel++;
            UpdateDescription();
        }
    }

    public void OnSelectionChanged(int id)
    {
        descriptionPanel.SetActive(id >= 0);
        UpdateDescription();
    }
}
