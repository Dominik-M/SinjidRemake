using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public enum Frame
    {
        WELCOME, MAIN, START, CHOOSE
    }

    public GameObject welcomeFrame, mainMenuFrame, startFrame, chooseCharacterFrame, optionsFrame;

    private Frame currentFrame;

    public Frame CurrentFrame
    {
        get => currentFrame; set
        {
            currentFrame = value;
            welcomeFrame.SetActive(currentFrame == Frame.WELCOME);
            mainMenuFrame.SetActive(currentFrame == Frame.MAIN);
            startFrame.SetActive(currentFrame == Frame.START);
            chooseCharacterFrame.SetActive(currentFrame == Frame.CHOOSE);
        }
    }


    void Start()
    {
        CurrentFrame = Frame.WELCOME;
    }

    public void OnWelcomeProceed()
    {
        Debug.Log("OnWelcomeProceed");
        CurrentFrame = Frame.MAIN;
    }
    public void OnStartProceed()
    {
        Debug.Log("OnStartProceed");
        CurrentFrame = Frame.CHOOSE;
    }

    public void OnNewGame()
    {
        Debug.Log("OnNewGame");
        CurrentFrame = Frame.START;
    }
    public void OnLoadGame()
    {
        Debug.Log("OnLoadGame");
        GameController.LoadAllPrefs();
        if (GameController.MyClass == CharacterClass.Invalid)
        {
            Debug.Log("No valid savegame available");
        }
        else
            GameController.LoadWorldScene();
    }

    public void OnChooseButton(int idx)
    {
        Debug.Log("OnChooseButton");
        CharacterClass chosen = (CharacterClass)idx;
        GameController.LoadWorldScene();
        GameController.InitCharacter(chosen);
    }

    public void OnToggleOptions()
    {
        Debug.Log("OnToggleOptions");
        optionsFrame.SetActive(!optionsFrame.activeInHierarchy);
    }

    public void OnClearData()
    {
        Debug.Log("OnClearData");
        PlayerPrefs.DeleteAll();
    }
}
