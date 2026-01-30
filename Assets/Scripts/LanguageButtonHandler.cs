using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonHandler : MonoBehaviour
{
    [Header("Language")]
    [SerializeField] private Localization.Language language;

    private Image mImg;

    void Start()
    {
        mImg = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
        Localization.OnLanguageChanged += OnChange;
        OnChange();
    }

    public void OnClick()
    {
        Localization.SetLanguage(language);
    }

    public void OnChange()
    {
        Color c = mImg.color;
        c.a = (Localization.CurrentLanguage == (int)language) ? 1:0;
        mImg.color = c;
    }

}
