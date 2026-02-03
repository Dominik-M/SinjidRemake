using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Localization
{
    public static readonly bool dumpTexts = false;

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
        for (int i = 0; i < translations.Length; i++)
        {
            translations[i] = new Dictionary<int, string>();
            Language l = ((Language)i);
            Debug.Log("Loading texts for language " + l);
            TextAsset[] textassets = Resources.LoadAll<TextAsset>("Texts/" + l.ToString());
            int count = 0;
            foreach (TextAsset text in textassets)
            {
                int id = int.Parse(text.name);
                //Debug.Log(text.name+": id= "+id+" content= "+text.text);
                translations[i].Add(id, text.text);
                count++;
            }
            Debug.Log("Found " + count + " texts");
        }

        if (dumpTexts)
        {
            Debug.Log("Dump Text Assets...");
            TextAsset[] texts = Resources.LoadAll<TextAsset>("Texts/EN");
            int n = 0;
            foreach (TextAsset text in texts)
            {
                int id = int.Parse(text.name);
                Debug.Log(text.name + ": id= " + id + " content= " + text.text);
                n++;
            }
            Debug.Log("Found " + n + " texts");
#if UNITY_EDITOR
            WriteCharacterNames(3000);
            WriteItemNames(3100);
            AssetDatabase.SaveAssets();
#endif
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
#if UNITY_EDITOR

    private static void WriteCharacterNames(int startIdx)
    {
        string folderpath = Path.Combine(Application.persistentDataPath, "Texts");

        if (!Directory.Exists(folderpath))
            Directory.CreateDirectory(folderpath);

        int i = 0;
        Character[] characters = Resources.LoadAll<Character>("Character");
        foreach (Character c in characters)
        {
            c.displayNameTextId = (i + startIdx);
            EditorUtility.SetDirty(c);
            string filepath = Path.Combine(folderpath, c.displayNameTextId + ".txt");
            File.WriteAllText(filepath, c.displayName);
            Debug.Log("Write file: " + filepath);
            i++;
        }
    }
    private static void WriteItemNames(int startIdx)
    {
        string folderpath = Path.Combine(Application.persistentDataPath, "Texts");

        if (!Directory.Exists(folderpath))
            Directory.CreateDirectory(folderpath);

        int i = 0;
        Item[] items = Resources.LoadAll<Item>("Items");
        foreach (Item c in items)
        {
            c.displaynameTextId = (i + startIdx);
            EditorUtility.SetDirty(c);
            string filepath = Path.Combine(folderpath, c.displaynameTextId + ".txt");
            File.WriteAllText(filepath, c.displayname);
            Debug.Log("Write file: " + filepath);
            i++;
            c.descriptionTextId = (i + startIdx);
            EditorUtility.SetDirty(c);
            filepath = Path.Combine(folderpath, c.descriptionTextId + ".txt");
            File.WriteAllText(filepath, c.description);
            Debug.Log("Write file: " + filepath);
            i++;
        }
    }
#endif

}
