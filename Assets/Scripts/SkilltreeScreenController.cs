using UnityEngine;
using UnityEngine.UI;

public class SkilltreeScreenController : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private Text skillpoints;
    [SerializeField] private Text level;

    void Update()
    {
        skillpoints.text = GameController.Skillpoints.ToString();
        level.text = GameController.Level.ToString();
    }

    public override void L1()
    {
        Debug.Log("InventoryScreenController.L1()");
        GameController.OpenInventory();
    }
    public override void R1()
    {
        Debug.Log("InventoryScreenController.R1()");
        GameController.OpenInventory();
    }
}
