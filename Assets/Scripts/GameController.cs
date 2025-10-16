using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Balancing")]
    [SerializeField] private float idleEngConsumption = 0.1f, movingEngConsumption = 1.0f, decreaseSpeed = 2.0f;

    [SerializeField] private ItemContainer allItems, shopItemsMarket, shopItemsArmory1, shopItemsArmory2, shopItemsMGate, shopItemsDGate;

    [Header("Screens")]
    [SerializeField] private GameObject restScreen, healScreen, foodScreen, potionScreen, inventoryScreen, shopScreen;

    [Header("UI Elements")]
    [SerializeField] private Text lifetext, maxlifetext;
    [SerializeField] private Slider lifeslider;
    [SerializeField] private Text manatext, maxmanatext;
    [SerializeField] private Slider manaslider;
    [SerializeField] private Text engtext, maxengtext;
    [SerializeField] private Slider engslider;
    [SerializeField] private Text exptext, expnexttext;
    [SerializeField] private Slider expslider;
    [SerializeField] private Text goldtext, leveltext;
    [SerializeField] private Text mClassText;
    [SerializeField] private GameObject infoDialog;
    [SerializeField] private Text infoDialogTitle, infoDialogMessage;

    [Header("Minimap")]
    [SerializeField] private GameObject mapArrowLeft, mapArrowRight, mapArrowUp, mapArrowDown;

    [Header("References")]
    [SerializeField] private PlayerController worldPlayer;

    [SerializeField] private Room[] allRooms;

    const float deadzone = 0.2f, digipadDebounceTime = 0.5f;
    private float digipadDebounce = 0f;
    private GameObject currentMenu;
    private GameObject currentRoomObject;
    private DefaultMenuButtonHandler currentMenuButtonHandler;

    void Start()
    {
        instance = this;
        LoadAllPrefs();
        if (currentRoom)
        {
            ChangeRoom(currentRoom);
        }
    }

    void Update()
    {
        // Get all inputs
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float digiPadHorizontal = Input.GetAxis("Right") - Input.GetAxis("Left");
        float digiPadVertical = Input.GetAxis("Up") - Input.GetAxis("Down");
        bool dreieck = Input.GetButtonDown("Dreieck");
        bool kreuz = Input.GetButtonDown("Kreuz");
        bool kreis = Input.GetButton("Kreis");
        bool kasten = Input.GetButtonDown("Kasten");
        bool r1 = Input.GetButtonDown("R1");
        bool l1 = Input.GetButtonDown("L1");
        bool start = Input.GetButtonDown("Start");

        // Menu navigation with digipad
        if (digipadDebounce > 0)
        {
            digipadDebounce -= Time.deltaTime;
        }
        else
        {
            // This is already done per default!
            //NavigateInMenu(digiPadHorizontal, digiPadVertical);
        }

        // Button handling in menu
        if (currentMenuButtonHandler)
        {
            if (kreis)
            {
                currentMenuButtonHandler.Kreis();
            }
            else if (kreuz)
            {
                // done by default
                // currentMenuButtonHandler.Kreuz();
            }
            else if (kasten)
            {
                currentMenuButtonHandler.Kasten();
            }
            else if (dreieck)
            {
                currentMenuButtonHandler.Dreieck();
            }
            else if (r1)
            {
                currentMenuButtonHandler.R1();
            }
            else if (l1)
            {
                currentMenuButtonHandler.L1();
            }
        }
        // Button handling for player in world
        else if (worldPlayer)
        {
            worldPlayer.Move(h, v);
            if (worldPlayer.IsMoving())
            {
                Eng -= movingEngConsumption * Time.deltaTime;
            }
            else
            {
                Eng -= idleEngConsumption * Time.deltaTime;
            }
            if (Eng <= 0)
            {
                float n = Life - decreaseSpeed * Time.deltaTime;
                Life = n < 1 ? 1 : n;
                n = Mana - decreaseSpeed * Time.deltaTime;
                Mana = n < 1 ? 1 : n;
            }
            if (start)
            {
                // open inventory
                if(inventoryScreen)
                    OpenMenu(inventoryScreen);
            }
            else if (kreuz)
            {
                // interaction
                Interaction();
            }
        }
    }

    private void NavigateInMenu(float h, float v)
    {
        if (currentMenuButtonHandler)
        {
            digipadDebounce = digipadDebounceTime;
            if (h > deadzone)
            {
                // Right
                currentMenuButtonHandler.Right();
            }
            else if (h < -deadzone)
            {
                // Left
                currentMenuButtonHandler.Left();
            }
            else if (v > deadzone)
            {
                // Down
                currentMenuButtonHandler.Down();
            }
            else if (v < -deadzone)
            {
                // Up
                currentMenuButtonHandler.Up();
            }
            else
            {
                digipadDebounce = 0;
            }
        }
    }

    private void Interaction()
    {
        Debug.Log("Interaction: " + currentInteraction);
        switch (currentInteraction)
        {
            case InteractionID.NONE:
                break;
            case InteractionID.EXIT_UP:
                ChangeRoom(currentRoom.up);
                if (worldPlayer)
                    worldPlayer.transform.position = new Vector3(worldPlayer.transform.position.x, -1.25f, worldPlayer.transform.position.z);
                break;
            case InteractionID.EXIT_DOWN:
                ChangeRoom(currentRoom.down);
                if (worldPlayer)
                    worldPlayer.transform.position = new Vector3(worldPlayer.transform.position.x, 1f, worldPlayer.transform.position.z);
                break;
            case InteractionID.EXIT_LEFT:
                ChangeRoom(currentRoom.left);
                if (worldPlayer)
                    worldPlayer.transform.position = new Vector3(3.5f, worldPlayer.transform.position.y, worldPlayer.transform.position.z);
                break;
            case InteractionID.EXIT_RIGHT:
                ChangeRoom(currentRoom.right);
                if (worldPlayer)
                    worldPlayer.transform.position = new Vector3(-3.5f, worldPlayer.transform.position.y, worldPlayer.transform.position.z);
                break;
            case InteractionID.ENTRANCE_NPC1:
                OpenMenu(restScreen);
                break;
            case InteractionID.ENTRANCE_NPC2:
                OpenMenu(healScreen);
                break;
            case InteractionID.ENTRANCE_NPC3:
                ShowInfoDialog("Student", "This is the arena entrance. If you are hurt, the man in the white robes will heal you for a cheap price. If you want to save, talk to the old man with the stick.");
                break;
            case InteractionID.BASAR_NPC1:
                break;
            case InteractionID.BASAR_NPC2:
                break;
            case InteractionID.BASAR_NPC3:
                break;
            case InteractionID.HGATE_GATE_ENTRY:
                // TODO start battle
                break;
            case InteractionID.HGATE_NPC1:
                break;
            case InteractionID.HGATE_NPC2:
                break;
            case InteractionID.MARKET_NPC1:
                break;
            case InteractionID.MARKET_NPC2:
                break;
            case InteractionID.MARKET_NPC3:
                break;
            case InteractionID.PUB_NPC1:
                break;
            case InteractionID.PUB_NPC2:
                break;
            case InteractionID.PUB_NPC3:
                break;
            case InteractionID.PUB_NPC4:
                break;
            case InteractionID.ARMORY_NPC1:
                break;
            case InteractionID.ARMORY_NPC2:
                break;
            case InteractionID.ARMORY_NPC3:
                break;
            case InteractionID.LIBRARY_NPC1:
                break;
            case InteractionID.LIBRARY_NPC2:
                break;
            case InteractionID.LIBRARY_NPC3:
                break;
            case InteractionID.LIBRARY_BOOKS:
                break;
            case InteractionID.SECRET:
                break;
            case InteractionID.MGATE_GATE_ENTRY:
                break;
            case InteractionID.MGATE_NPC1:
                break;
            case InteractionID.MGATE_NPC2:
                break;
            case InteractionID.MGATE_NPC3:
                break;
            case InteractionID.MGATE_NOTE:
                break;
            case InteractionID.TRAINING_ENTRY:
                break;
            case InteractionID.PREDARK_NPC1:
                break;
            case InteractionID.PREDARK_NPC2:
                break;
            case InteractionID.DGATE_NPC1:
                break;
            case InteractionID.DGATE_NPC2:
                break;
            case InteractionID.DGATE_NPC3:
                break;
            case InteractionID.DGATE_GATE_ENTRY:
                break;
        }
    }

    public void OpenMenu(GameObject menu)
    {
        currentMenu = menu;
        currentMenuButtonHandler = menu.GetComponent<DefaultMenuButtonHandler>();
        currentMenu.SetActive(true);
    }

    public void ShowInfoDialog(string title, string message)
    {
        OpenMenu(infoDialog);
        infoDialogTitle.text = title;
        infoDialogMessage.text = message;
    }

    public void ChangeRoom(Room next)
    {
        if (next)
        {
            Debug.Log("ChangeRoom: " + next.id);

            if (currentRoomObject)
                Destroy(currentRoomObject);

            currentRoom = next;
            currentRoomObject = Instantiate(next.prefab);

            mapArrowLeft.SetActive(next.left != null);
            mapArrowRight.SetActive(next.right != null);
            mapArrowUp.SetActive(next.up != null);
            mapArrowDown.SetActive(next.down != null);
        }
        else
        {
            Debug.LogWarning("ChangeRoom: Invalid Room!");
        }
    }

    // Global Functions

    // refs
    private static GameController instance;
    private static InteractionID currentInteraction;
    private static Room currentRoom;

    // stats
    private static float life, maxlife;
    private static float mana, maxmana;
    private static float eng, maxeng;
    private static float exp, expNext;
    private static int gold, level, lifePotions, manaPotions;
    private static int strength, dex, magic;
    private static CharacterClass mClass;

    // items
    private static Item[] inventoryItems = new Item[8];
    private static Item weapon, shield, suit, headgear;
    private static ItemContainer currentShopItems = null;

    public static Item GetInventoryItem(int idx)
    {
        if (idx >= 0 && idx < inventoryItems.Length)
            return inventoryItems[idx];
        return null;
    }
    public static void SetInventoryItem(int idx, Item item)
    {
        if (idx >= 0 && idx < inventoryItems.Length)
            inventoryItems[idx] = item;
    }
    public static Item GetShopItem(int idx)
    {
        if ((currentShopItems != null)
            && (idx >= 0 && idx < currentShopItems.items.Length))
            return currentShopItems.items[idx];
        return null;
    }

    public static Item Weapon { get => weapon; set => weapon = value; }
    public static Item Shield { get => shield; set => shield = value; }
    public static Item Suit { get => suit; set => suit = value; }
    public static Item Headgear { get => headgear; set => headgear = value; }

    public static InteractionID CurrentInteraction
    {
        get => currentInteraction; set => currentInteraction = value;
    }

    public static Room FindRoomByID(RoomID id)
    {
        if (instance)
        {
            foreach (Room r in instance.allRooms)
                if (r.id == id)
                    return r;
        }
        return null;
    }

    public static CharacterClass MyClass
    {
        get => mClass; set
        {
            mClass = value;
            if (instance && instance.mClassText)
                instance.mClassText.text = mClass.ToString();
        }

    }

    public static int Gold
    {
        get => gold; set
        {
            gold = value;
            if (instance && instance.goldtext)
                instance.goldtext.text = "Gold: " + gold;
        }
    }
    public static int Level
    {
        get => level; set
        {
            level = value;
            if (instance && instance.leveltext)
                instance.leveltext.text = "Level " + level;
        }
    }
    public static int LifePotions
    {
        get => lifePotions; set
        {
            lifePotions = value;
        }
    }
    public static int ManaPotions
    {
        get => manaPotions; set
        {
            manaPotions = value;
        }
    }
    public static int Strength
    {
        get => strength; set
        {
            strength = value;
        }
    }
    public static int Dex
    {
        get => dex; set
        {
            dex = value;
        }
    }
    public static int Magic
    {
        get => magic; set
        {
            magic = value;
        }
    }

    public static int PhysDmg => Strength + Dex; // TODO use weapon scaling
    public static int MagicDmg => Magic;
    public static int PhysDef => Strength; // TODO add armor
    public static int MagicDef => Magic;

    public static float GetDodgeChance()
    {
        float percent;
        if (Dex < 20)
            percent = Dex * 1.0f; // max 20%
        else if (Dex < 40)
            percent = 10 + Dex * 0.5f; // max 30%
        else if (Dex < 100)
            percent = 18 + Dex * 0.3f; // max 48%
        else if (Dex < 198)
            percent = 28 + Dex * 0.2f; // max 68%
        else
            percent = 68.0f;

        return percent/100.0f;
    }

    public static float Life
    {
        get => life; set
        {
            life = value;
            if (life <= 0)
                Die();
            if (life > maxlife)
                life = maxlife;
            if (instance && instance.lifetext)
                instance.lifetext.text = life.ToString("F0");
            if (instance && instance.lifeslider)
                instance.lifeslider.value = life / maxlife;
        }
    }
    public static float MaxLife
    {
        get => maxlife; set
        {
            maxlife = value;
            if (instance && instance.maxlifetext)
                instance.maxlifetext.text = maxlife.ToString("F0");
            if (instance && instance.lifeslider)
                instance.lifeslider.value = life / maxlife;
        }
    }
    public static float Mana
    {
        get => mana; set
        {
            mana = value;
            if (mana < 0)
                mana = 0;
            if (mana > maxmana)
                mana = maxmana;
            if (instance && instance.manatext)
                instance.manatext.text = mana.ToString("F0");
            if (instance && instance.manaslider)
                instance.manaslider.value = mana / maxmana;
        }
    }
    public static float MaxMana
    {
        get => maxmana; set
        {
            maxmana = value;
            if (instance && instance.maxmanatext)
                instance.maxmanatext.text = maxmana.ToString("F0");
            if (instance && instance.manaslider)
                instance.manaslider.value = mana / maxmana;
        }
    }
    public static float Eng
    {
        get => eng; set
        {
            eng = value;
            if (eng < 0)
                eng = 0;
            if (eng > maxeng)
                eng = maxeng;
            if (instance && instance.engtext)
                instance.engtext.text = eng.ToString("F0");
            if (instance && instance.engslider)
                instance.engslider.value = eng / maxeng;
        }
    }
    public static float MaxEng
    {
        get => maxeng; set
        {
            maxeng = value;
            if (instance && instance.maxengtext)
                instance.maxengtext.text = maxeng.ToString("F0");
            if (instance && instance.engslider)
                instance.engslider.value = eng / maxeng;
        }
    }
    public static float Exp
    {
        get => exp; set
        {
            exp = value;
            if (eng < 0)
                eng = 0;
            if (exp >= expNext)
                LevelUp();
            if (instance && instance.exptext)
                instance.exptext.text = exp.ToString("F0");
            if (instance && instance.expslider)
                instance.expslider.value = exp / ExpNext;
        }
    }
    public static float ExpNext
    {
        get => expNext; set
        {
            expNext = value;
            if (instance && instance.expnexttext)
                instance.expnexttext.text = expNext.ToString("F0");
            if (instance && instance.expslider)
                instance.expslider.value = exp / expNext;
        }
    }

    public static void InitCharacter(CharacterClass c)
    {
        Debug.Log("InitCharacter Class: " + c);
        MyClass = c;
        Gold = 100;
        Level = 1;
        ExpNext = 40;
        Exp = 0;
        LifePotions = 5;
        ManaPotions = 5;
        MaxEng = 50;
        Eng = 50;
        currentRoom = FindRoomByID(RoomID.ENTRANCE);
        switch (c)
        {
            case CharacterClass.Balanced:
                MaxLife = 100;
                Life = 100;
                MaxMana = 80;
                Mana = 80;
                Strength = 3;
                Dex = 3;
                Magic = 3;
                break;
            case CharacterClass.Warrior:
                MaxLife = 120;
                Life = 120;
                MaxMana = 40;
                Mana = 40;
                Strength = 5;
                Dex = 2;
                Magic = 2;
                break;
            case CharacterClass.Spellcaster:
                MaxLife = 60;
                Life = 60;
                MaxMana = 120;
                Mana = 120;
                Strength = 2;
                Dex = 2;
                Magic = 5;
                break;
            case CharacterClass.Ninja:
                MaxLife = 80;
                Life = 80;
                MaxMana = 80;
                Mana = 80;
                Strength = 3;
                Dex = 5;
                Magic = 3;
                break;
        }
        SaveAllPrefs();
    }

    private static void Die()
    {
        life = 0;
        // TODO gameover screen
    }

    private static void LevelUp()
    {
        exp = 0;
        ExpNext = expNext + expNext * 0.25f;
        Level++;
    }

    public static void LoadWorldScene()
    {
        SceneManager.LoadScene(1);
    }

    public static void CloseMenu()
    {
        if (instance && instance.currentMenu)
        {
            instance.currentMenu.SetActive(false);
            instance.currentMenu = null;
            instance.currentMenuButtonHandler = null;
        }
        currentShopItems = null;
    }

    public static void LoadAllPrefs()
    {
        Debug.Log("LoadAllPrefs()");
        MyClass = (CharacterClass)(PlayerPrefs.GetInt("class", (int)CharacterClass.Invalid));
        Gold = PlayerPrefs.GetInt("gold", 100);
        Level = PlayerPrefs.GetInt("level", 1);
        MaxLife = PlayerPrefs.GetInt("maxlife", 80);
        Life = PlayerPrefs.GetInt("life", 80);
        MaxMana = PlayerPrefs.GetInt("maxmana", 60);
        Mana = PlayerPrefs.GetInt("mana", 60);
        MaxEng = PlayerPrefs.GetInt("maxeng", 50);
        Eng = PlayerPrefs.GetInt("eng", 50);
        ExpNext = PlayerPrefs.GetInt("expNext", 40);
        Exp = PlayerPrefs.GetInt("exp", 0);
        Strength = PlayerPrefs.GetInt("strength", 2);
        Dex = PlayerPrefs.GetInt("dex", 2);
        Magic = PlayerPrefs.GetInt("magic", 2);
        LifePotions = PlayerPrefs.GetInt("lifepotions", 0);
        ManaPotions = PlayerPrefs.GetInt("manapotions", 0);
        int roomid = PlayerPrefs.GetInt("room", (int)(RoomID.ENTRANCE));
        currentRoom = FindRoomByID((RoomID)roomid);
    }

    public static void SaveAllPrefs()
    {
        Debug.Log("SaveAllPrefs()");
        PlayerPrefs.SetInt("class", (int)MyClass);
        PlayerPrefs.SetInt("gold", Gold);
        PlayerPrefs.SetInt("level", Level);
        PlayerPrefs.SetInt("maxlife", (int)MaxLife);
        PlayerPrefs.SetInt("life", (int)Life);
        PlayerPrefs.SetInt("maxmana", (int)MaxMana);
        PlayerPrefs.SetInt("mana", (int)Mana);
        PlayerPrefs.SetInt("maxeng", (int)MaxEng);
        PlayerPrefs.SetInt("eng", (int)Eng);
        PlayerPrefs.SetInt("expNext", (int)ExpNext);
        PlayerPrefs.SetInt("exp", (int)Exp);
        PlayerPrefs.SetInt("strength", Strength);
        PlayerPrefs.SetInt("dex", Dex);
        PlayerPrefs.SetInt("magic", Magic);
        PlayerPrefs.SetInt("lifepotions", LifePotions);
        PlayerPrefs.SetInt("manapotions", ManaPotions);
        if (currentRoom)
            PlayerPrefs.SetInt("room", (int)currentRoom.id);
        else
            PlayerPrefs.SetInt("room", (int)RoomID.ENTRANCE);
    }
}
