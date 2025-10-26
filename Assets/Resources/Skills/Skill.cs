using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public CombatAction associatedAction;
    public string displayname;
    public string description;
    public Sprite icon;
    public Skill[] prerequisites;
    public int requiredLevel;
    public bool passive;
    public int manacost;
    public int currentlevel;
    public int[] levelvalues;
    public string effectTitle;
    public string effectValueDescription;

    public int GetLevelValue(int level)
    {
        if (level <= 0)
        {
            return 0;
        }
        else if (levelvalues.Length < level)
        {
            return 0;
        }
        else
        {
            return levelvalues[level - 1];
        }
    }
    public int GetCurrentLevelValue()
    {
        return GetLevelValue(currentlevel);
    }
    public string GetLevelValueDescription(int level)
    {
        int value = GetLevelValue(level);
        if (value == 0)
            return "-";
        return value + effectValueDescription;
    }

    public string GetCurrentLevelValueDescription()
    {
        return GetLevelValueDescription(currentlevel);
    }
    public string GetNextLevelValueDescription()
    {
        return GetLevelValueDescription(currentlevel + 1);
    }

    public bool CanUpgrade()
    {
        if (GameController.Level < requiredLevel)
            return false;
        if (currentlevel >= levelvalues.Length)
            return false;
        foreach (Skill prev in prerequisites)
            if (prev.currentlevel <= 0)
                return false;
        return true;
    }
}
