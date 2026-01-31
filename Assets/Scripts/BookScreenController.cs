using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BookScreenController : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject bookshelfScreen;
    [SerializeField] private GameObject bookContentScreen;
    [SerializeField] private Text bookTitle, bookContent;
    [SerializeField] private Button bookBackButton;

    private Selectable lastselected;

    void OnEnable()
    {
        bookshelfScreen.SetActive(true);
        bookContentScreen.SetActive(false);
    }

    public void OnBookButton(int idx)
    {
        bookshelfScreen.SetActive(false);
        bookContentScreen.SetActive(true);
        switch (idx)
        {
            case 0:
                bookTitle.text = Localization.GetText(1363);
                bookContent.text = Localization.GetText(1375);
                break;
            case 1:
                bookTitle.text = Localization.GetText(1364);
                bookContent.text = Localization.GetText(1377);
                break;
            case 2:
                bookTitle.text = Localization.GetText(1367);
                bookContent.text = Localization.GetText(1379);
                break;
            case 3:
                bookTitle.text = Localization.GetText(1368);
                bookContent.text = Localization.GetText(1381);
                break;
        }
        lastselected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        bookBackButton.Select();
    }

    public void OnBookClose()
    {
        bookshelfScreen.SetActive(true);
        bookContentScreen.SetActive(false);
        if(lastselected)
            lastselected.Select();
    }
}
