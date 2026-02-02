using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public enum Frame
    {
        WELCOME, MAIN, START, CHOOSE, LOADING
    }

    public GameObject welcomeFrame, mainMenuFrame, startFrame, chooseCharacterFrame, loadingScreen;
    public Text versionNumber;
    public DefaultMenuButtonHandler menuButtonHandler;

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

    void Update()
    {
        // Get all Gamepad inputs
        bool dreieck = Input.GetButtonDown("Dreieck");
        bool kreis = Input.GetButton("Kreis");
        bool kasten = Input.GetButtonDown("Kasten");
        bool r1 = Input.GetButtonDown("R1");
        bool l1 = Input.GetButtonDown("L1");

        // Button handling in menu
        // kreuz is handled by event system as default submit button
        if (menuButtonHandler != null)
        {
            if (kreis)
            {
                menuButtonHandler.Kreis();
            }
            else if (kasten)
            {
                menuButtonHandler.Kasten();
            }
            else if (dreieck)
            {
                menuButtonHandler.Dreieck();
            }
            else if (r1)
            {
                menuButtonHandler.R1();
            }
            else if (l1)
            {
                menuButtonHandler.L1();
            }
        }
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
