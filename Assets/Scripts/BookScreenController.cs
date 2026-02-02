using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BookScreenController : DefaultMenuButtonHandler
{

    [Header("UI References")]
    [SerializeField] private GameObject bookshelfScreen;
    [SerializeField] private GameObject bookContentScreen;
    [SerializeField] private Text bookTitle, bookContent;
    [SerializeField] private Button bookBackButton;

    private Selectable lastselected;
    private static bool bookOpen, book1read = false, book2read = false, book3read = false, book4read = false;

    public override void OnEnable()
    {
        bookshelfScreen.SetActive(true);
        bookContentScreen.SetActive(false);
    }

    public void OnBookButton(int idx)
    {
        bookOpen = true;
        switch (idx)
        {
            case 0:
                bookTitle.text = Localization.GetText(1363);
                bookContent.text = Localization.GetText(1375);
                book1read = true;
                break;
            case 1:
                bookTitle.text = Localization.GetText(1364);
                bookContent.text = Localization.GetText(1377);
                book2read = true;
                break;
            case 2:
                bookTitle.text = Localization.GetText(1367);
                bookContent.text = Localization.GetText(1379);
                book3read = true;
                break;
            case 3:
                bookTitle.text = Localization.GetText(1368);
                bookContent.text = Localization.GetText(1381);
                book4read = true;
                break;
            default:
                // no book found so unset the bookopen flag
                bookOpen = false;
                break;
        }
        if (bookOpen)
        {
            bookshelfScreen.SetActive(false);
            bookContentScreen.SetActive(true);
            lastselected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            bookBackButton.Select();
        }
    }

    public void OnBookClose()
    {
        bookshelfScreen.SetActive(true);
        bookContentScreen.SetActive(false);
        if (lastselected)
            lastselected.Select();
    }

    public override void Kreis()
    {
        if (closeWithKreis)
        {
            if (bookOpen)
            {
                OnBookClose();
            }
            else
            {
                OnBookshelfExit();
            }
        }
    }

    public void OnBookshelfExit()
    {
        GameController.CloseMenu();
        if (book1read && book2read && book3read && book4read)
            GameController.OnAllBooksRead();
    }
}
