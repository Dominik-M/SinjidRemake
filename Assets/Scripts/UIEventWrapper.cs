using UnityEngine;

public class UIEventWrapper : MonoBehaviour
{
    public void OnLifepotionClicked()
    {
        GameController.UseLifePotion();
    }
    public void OnManapotionClicked()
    {
        GameController.UseManaPotion();
    }
    public void OnYesClicked()
    {
        GameController.OnYesClicked();
    }
    public void OnNoClicked()
    {
        GameController.OnNoClicked();
    }
}
