using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [Header("Text ID")]
    [SerializeField] private int id;
    [Header("Auto update text on language change")]
    [SerializeField] private bool listenOnChange=false;

    private Text mText;

    void Start()
    {
        mText = GetComponent<Text>();
        mText.text = Localization.GetText(id);
        if (string.IsNullOrEmpty(mText.text))
            Debug.LogWarning("Text invalid at " + gameObject);
        if (listenOnChange)
            Localization.OnLanguageChanged += OnLanguageChanged;
    }

    public void OnLanguageChanged()
    {
        mText.text = Localization.GetText(id);
    }

}
