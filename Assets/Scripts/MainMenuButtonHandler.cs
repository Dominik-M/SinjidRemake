using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonHandler : DefaultMenuButtonHandler
{
    public GameObject optionsFrame;
    public Selectable defaultOptionButton;

    public override void Kreis()
    {
        OnCloseOptions();
    }

    public void OnOpenOptions()
    {
        Debug.Log("OnToggleOptions");
        optionsFrame.SetActive(true);
        if (defaultOptionButton != null)
            defaultOptionButton.Select();
    }

    public void OnCloseOptions()
    {
        Debug.Log("OnToggleOptions");
        optionsFrame.SetActive(false);
        defaultbutton.Select();
    }
}
