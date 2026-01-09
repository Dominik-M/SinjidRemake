using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{
    public static readonly int DEFAULT_WIDTH = 600, DEFAULT_HEIGHT = 460;
    public static readonly float FACTOR_S = 0.75f, FACTOR_L = 1.5f, FACTOR_XL = 2.0f, FACTOR_XXL = 3.0f;

    public Dropdown resDropdown;

    public void Start()
    {
        int i = LoadPref();
        resDropdown.value = i;
        SetScreenResolution(i);
    }

    public void OnDropdownValueChanged()
    {
        int value = resDropdown.value;
        Debug.Log("OnDropdownValueChanged: "+value);
        SetScreenResolution(value);
        SavePref(value);
    }

    public void SetScreenResolution(int idx)
    {

        switch (idx)
        {
            case 0:
                //small
                Screen.SetResolution((int)(DEFAULT_WIDTH * FACTOR_S), (int)(DEFAULT_HEIGHT * FACTOR_S), false);
                break;
            case 1:
                // normal
                Screen.SetResolution(DEFAULT_WIDTH, DEFAULT_HEIGHT, false);
                break;
            case 2:
                // large
                Screen.SetResolution((int)(DEFAULT_WIDTH * FACTOR_L), (int)(DEFAULT_HEIGHT * FACTOR_L), false);
                break;
            case 3:
                // extra large
                Screen.SetResolution((int)(DEFAULT_WIDTH * FACTOR_XL), (int)(DEFAULT_HEIGHT * FACTOR_XL), false);
                break;
            case 4:
                // XXL
                Screen.SetResolution((int)(DEFAULT_WIDTH * FACTOR_XXL), (int)(DEFAULT_HEIGHT * FACTOR_XXL), false);
                break;
        }
    }

    public int LoadPref()
    {
        return PlayerPrefs.GetInt("ScreenResolution", 1);
    }
    public void SavePref(int value)
    {
        PlayerPrefs.SetInt("ScreenResolution", value);
    }
}
