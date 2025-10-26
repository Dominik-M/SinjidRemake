using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text physdmg;
    [SerializeField] private Text physdef;
    [SerializeField] private Text magicdmg;
    [SerializeField] private Text magicdef;
    [SerializeField] private Text characterclass;
    [SerializeField] private Text level;
    [SerializeField] private Text strength;
    [SerializeField] private Text dex;
    [SerializeField] private Text magic;

    void Update()
    {
        if (level)
            level.text = "Level "+GameController.Level.ToString();
        if (characterclass)
            characterclass.text = GameController.MyClass.ToString();
        if (physdmg)
            physdmg.text = "Physical   Damage:    " + GameController.GetPhysDmg();
        if (physdef)
            physdef.text = "Physical   Defence:    " + GameController.GetPhysDef();
        if (magicdmg)
            magicdmg.text = "Magical   Damage:    " + GameController.GetMagicDmg();
        if (magicdef)
            magicdef.text = "Magical   Defence:    " + GameController.GetMagicDef();
        if (strength)
            strength.text = GameController.Strength.ToString();
        if (dex)
            dex.text = GameController.Dex.ToString();
        if (magic)
            magic.text = GameController.Magic.ToString();
    }
}
