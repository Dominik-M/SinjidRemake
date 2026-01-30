using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public enum Frame
    {
        WELCOME, MAIN, START, CHOOSE, LOADING
    }

    public GameObject welcomeFrame, mainMenuFrame, startFrame, chooseCharacterFrame, optionsFrame, loadingScreen;
    public Text versionNumber;

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
            loadingScreen.SetActive(currentFrame == Frame.LOADING);
        }
    }


    void Start()
    {
        Localization.Init();
        CurrentFrame = Frame.WELCOME;
        versionNumber.text = "v"+Application.version;
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
        Invoke(nameof(LoadWorldScene), 0.1f); // Delay the scene switch to show loading screen before
        GameController.InitCharacter(chosen);
        CurrentFrame = Frame.LOADING;
    }

    private void LoadWorldScene()
    {
        GameController.LoadWorldScene();
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

    public void OnViewIntro()
    {
        Debug.Log("OnViewIntro");
        CurrentFrame = Frame.WELCOME;
    }
}
