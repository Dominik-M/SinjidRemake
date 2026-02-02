using UnityEngine;
using System.Collections.Generic;
using System;

public class Localization
{
    [Header("Log all available original text resources")]
    private static readonly bool debugLog = false;

    public static event Action OnLanguageChanged;
    public enum Language
    {
        EN, DE, VN, COUNT
    }

    private static readonly Dictionary<int, string>[] translations = new Dictionary<int, string>[((int)Language.COUNT)];
    private static int currentLanguage;

    public static int CurrentLanguage
    {
        get => currentLanguage; set
        {
            if (currentLanguage != value && value < (int)Language.COUNT)
            {
                currentLanguage = value;
                Debug.Log("Set Language: " + currentLanguage);
                PlayerPrefs.SetInt("Language", CurrentLanguage);
                OnLanguageChanged?.Invoke();
            }
        }
    }

    public static void Init()
    {
        CurrentLanguage = PlayerPrefs.GetInt("Language", ((int)Language.EN));
        for(int i=0; i<translations.Length; i++)
        {
            translations[i] = new Dictionary<int, string>();
            Language l = ((Language)i);
            Debug.Log("Loading texts for language " + l);
            TextAsset[] texts = Resources.LoadAll<TextAsset>("Texts/" + l.ToString());
            int n = 0;
            foreach(TextAsset text in texts)
            {
                int id = int.Parse(text.name);
                //Debug.Log(text.name+": id= "+id+" content= "+text.text);
                translations[i].Add(id, text.text);
                n++;
            }
            Debug.Log("Found "+n+" texts");
        }
        if (debugLog)
        {
            Debug.Log("Dump Text Assets...");
            TextAsset[] texts = Resources.LoadAll<TextAsset>("Texts/Original");
            int n = 0;
            foreach (TextAsset text in texts)
            {
                int id = int.Parse(text.name);
                Debug.Log(text.name+": id= "+id+" content= "+text.text);
                n++;
            }
            Debug.Log("Found " + n + " texts");
        }
    }

    public static void SetLanguage(Language l)
    {
        CurrentLanguage = ((int)l);
    }

    public static string GetText(int id)
    {
        string text = "NA";
        translations[currentLanguage].TryGetValue(id, out text);
        if (string.IsNullOrEmpty(text))
            Debug.LogWarning("Translation not found for Text Id= " + id);
        return text;
    }
}
