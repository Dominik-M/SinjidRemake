using UnityEngine;
using UnityEngine.UI;

public class FoodScreenController : DefaultMenuButtonHandler
{
    [Header("References")]
    [SerializeField] private Text life;
    [SerializeField] private Text maxlife;
    [SerializeField] private Slider lifebar;
    [SerializeField] private AnimatedImage lifebarImage;
    [SerializeField] private Text mana;
    [SerializeField] private Text maxmana;
    [SerializeField] private Slider manabar;
    [SerializeField] private AnimatedImage manabarImage;
    [SerializeField] private Text eng;
    [SerializeField] private Text maxeng;
    [SerializeField] private Slider engbar;
    [SerializeField] private AnimatedImage engbarImage;
    [SerializeField] private Text gold;
    [SerializeField] private SelectionCallbackHandler[] buttons;
    [SerializeField] private GameObject[] descriptions;
    [SerializeField] private bool alwaysShowHints;

    void Start()
    {
        foreach (SelectionCallbackHandler button in buttons)
            button.OnSelectItem += OnButtonSelected;
    }

    public override void OnEnable()
    {
        Debug.Log("FoodScreenController.OnEnable()");
        base.OnEnable();
        UpdateValues();
    }

    void UpdateValues()
    {
        // Life
        float n = GameController.Life;
        float mn = GameController.MaxLife;
        life.text = n.ToString("F0");
        maxlife.text = mn.ToString("F0");
        lifebar.value = n / mn;
        // Mana
        n = GameController.Mana;
        mn = GameController.MaxMana;
        mana.text = n.ToString("F0");
        maxmana.text = mn.ToString("F0");
        manabar.value = n / mn;
        // Eng
        n = GameController.Eng;
        mn = GameController.MaxEng;
        eng.text = n.ToString("F0");
        maxeng.text = mn.ToString("F0");
        engbar.value = n / mn;
        // Gold
        gold.text = Localization.GetText(1296) + GameController.Gold;
    }

    public void OnButtonSelected(int idx)
    {
        for (int i = 0; i < descriptions.Length; i++)
            descriptions[i].SetActive((idx == i)||alwaysShowHints);
    }

    public void OnButtonClicked(int idx)
    {
        switch (idx)
        {
            case 0:
                BuyFood(5, 20, 5, 5);
                break;
            case 1:
                BuyFood(15, 50, 15, 15);
                break;
            case 2:
                BuyFood(20, 60, 20, 20);
                break;
            case 3:
                BuyFood(1, 2, 2, 2);
                break;
        }
    }

    private void BuyFood(int price,int ep, int hp, int mp)
    {
        if(price <= GameController.Gold)
        {
            GameController.Gold -= price;
            GameController.Eng += ep;
            GameController.Life += hp;
            GameController.Mana += mp;
            UpdateValues();
            lifebarImage.Restart();
            manabarImage.Restart();
            engbarImage.Restart();
        }
    }
}
