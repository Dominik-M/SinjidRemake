using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DefaultMenuButtonHandler : MonoBehaviour
{
    public Button defaultbutton;
    public bool closeWithKreis = true;

    public virtual void OnEnable()
    {
        defaultbutton.Select();
    }
    public virtual void Kreuz()
    {
        Debug.Log("DefaultMenuButtonHandler.Kreuz()");
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null)
        {
            Button selectedButton = selected.GetComponent<Button>();
            selectedButton.onClick.Invoke();
        }
    }
    public virtual void Kreis()
    {
        Debug.Log("DefaultMenuButtonHandler.Kreis()");
        if(closeWithKreis)
            GameController.CloseMenu();
    }
    public virtual void Kasten()
    {
        Debug.Log("DefaultMenuButtonHandler.Kasten()");
    }
    public virtual void Dreieck()
    {
        Debug.Log("DefaultMenuButtonHandler.Dreieck()");
    }

    public virtual void R1()
    {
        Debug.Log("DefaultMenuButtonHandler.R1()");
    }

    public virtual void L1()
    {
        Debug.Log("DefaultMenuButtonHandler.L1()");
    }
}
