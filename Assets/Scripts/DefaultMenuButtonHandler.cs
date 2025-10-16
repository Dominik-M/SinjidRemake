using UnityEngine;
using UnityEngine.UI;

public class DefaultMenuButtonHandler : MonoBehaviour
{
    public Button[] buttons;
    public int rowlength = 1;
    public bool closeWithKreis = true;

    private int idx;

    public int Idx
    {
        get => idx; set
        {
            idx = value;
            if (idx >= 0 && idx < buttons.Length)
                buttons[idx].Select();
        }
    }

    void Start()
    {
        Idx = 0;
    }
    public virtual void Up()
    {
        Debug.Log("DefaultMenuButtonHandler.Up()");
        if(Idx-rowlength >= 0)
            Idx -= rowlength;
    }
    public virtual void Down()
    {
        Debug.Log("DefaultMenuButtonHandler.Down()");
        if (Idx + rowlength < buttons.Length)
            Idx += rowlength;
    }
    public virtual void Left()
    {
        Debug.Log("DefaultMenuButtonHandler.Left()");
        if (Idx - 1 >= 0)
            Idx --;
    }
    public virtual void Right()
    {
        Debug.Log("DefaultMenuButtonHandler.Right()");
        if (Idx + 1 < buttons.Length)
            Idx++;
    }
    public virtual void Kreuz()
    {
        Debug.Log("DefaultMenuButtonHandler.Kreuz()");
        if (Idx >= 0 && Idx < buttons.Length)
            buttons[Idx].onClick.Invoke();
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
