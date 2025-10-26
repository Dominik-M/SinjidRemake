using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Stage")]
public class Stage : ScriptableObject
{
    public string title;
    public int currentLevel;
    public Character[] primaryEnemyPerStage;
    public Character[] secondaryEnemyPerStage;

    public Character[] GetCurrentStageCharacters()
    {
        Character[] retval = new Character[2];
        if (currentLevel >= 0 && currentLevel < primaryEnemyPerStage.Length)
        {
            retval[0] = primaryEnemyPerStage[currentLevel];
        }
        if (currentLevel >= 0 && currentLevel < secondaryEnemyPerStage.Length)
        {
            retval[1] = secondaryEnemyPerStage[currentLevel];
        }
        return retval;
    }

    public bool isFinished()
    {
        return currentLevel >= primaryEnemyPerStage.Length;
    }
}
