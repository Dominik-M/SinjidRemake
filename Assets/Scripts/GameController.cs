using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    [Header("Balancing")]
    public static readonly int StatPointsPerLevel = 2;
    [SerializeField] private float idleEngConsumption = 0.1f;
    [SerializeField] private float movingEngConsumption = 1.0f;
    [SerializeField] private float decreaseSpeed = 2.0f;

    [Header("Screens")]
    [SerializeField] private GameObject bottomview;
    [SerializeField] private GameObject restScreen;
    [SerializeField] private GameObject healScreen;
    [SerializeField] private GameObject foodScreen;
    [SerializeField] private GameObject potionScreen;
    [SerializeField] private GameObject inventoryScreen;
    [SerializeField] private GameObject skillsMenu;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject herbguyScreen;
    [SerializeField] private GameObject gambleScreen;
    [SerializeField] private GameObject trainingScreen;
    [SerializeField] private GameObject bookshelfScreen;
    [SerializeField] private GameObject endScreen;

    [Header("UI Elements")]
    [SerializeField] private GameObject infoDialog;
    [SerializeField] private GameObject yesnoDialog;
    [SerializeField] private Text infoDialogTitle, infoDialogMessage;
    [SerializeField] private Text yesnoDialogTitle, yesnoDialogMessage;
    [SerializeField] private Text lifepotionstext, manapotionstext;
    [SerializeField] private Text stageTitle, stageLevel;
    [SerializeField] private GameObject combatIntroScreen;
    [SerializeField] private SimpleTouchPad touchPad;

    [Header("Minimap")]
    [SerializeField] private GameObject mapArrowLeft;
    [SerializeField] private GameObject mapArrowRight;
    [SerializeField] private GameObject mapArrowUp;
    [SerializeField] private GameObject mapArrowDown;

    [Header("References")]
    [SerializeField] private PlayerController worldPlayer;
    [SerializeField] private Item herbsItem, blackHerbsItem;
    [SerializeField] private Item medicineItem, improvedMedicineItem, specialMedicineItem;
    [SerializeField] private Item secretringItem;
    [SerializeField] private Sprite chariconBalanced, chariconWarrior, chariconSpellcaster, chariconNinja;

    // internal fields
    private GameObject currentMenu;
    private GameObject currentRoomObject;

    void Start()
    {
        instance = this;
        combatIntroScreen.SetActive(false);
        if (!resourcesLoaded)
        {
            LoadResources();
        }
        if (shallInit)
        {
            // Started new game
            prestigeLevel = 0;
            InitCharacter();
            shallInit = false;
        }
        else
        {
            LoadAllPrefs();
        }
        switch (mClass)
        {
            case CharacterClass.Warrior:
                playerchar.icon = chariconWarrior;
                break;
            case CharacterClass.Spellcaster:
                playerchar.icon = chariconSpellcaster;
                break;
            case CharacterClass.Ninja:
                playerchar.icon = chariconNinja;
                break;
            case CharacterClass.Balanced:
            case CharacterClass.Invalid:
                playerchar.icon = chariconBalanced;
                break;
        }
        if (currentRoom)
        {
            ChangeRoom(currentRoom);
        }
        //debug
        //Skillpoints += 20;
    }

    void Update()
    {
        float h = 0, v = 0;
        // add touchpad inputs
        Vector2 direction = touchPad.GetDirection();
        h += direction.x;
        v += direction.y;
        //Debug.Log("h=" + h + " v=" + v);

        if (worldPlayer)
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
        }
    }

    public void OnInteractPressed()
    {
        Interaction();
    }

    private void Interaction()
    {
        Debug.Log("Interaction: " + currentInteraction);
        switch (currentInteraction)
        {
            case InteractionID.NONE:
                break;
            case InteractionID.PREDARK_EXIT:
                if (humangatewaystage.isFinished())
                {
                    ChangeRoom(currentRoom.up);
                    if (worldPlayer)
                        worldPlayer.transform.position = new Vector3(worldPlayer.transform.position.x, -1.25f, worldPlayer.transform.position.z);

                }
                else
                {
                    ShowInfoDialog(Localization.GetText(10), Localization.GetText(11));
                }
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
            case InteractionID.ENTRANCE_NPC1:// Opa
                OpenMenu(restScreen);
                break;
            case InteractionID.ENTRANCE_NPC2:// Healer
                OpenMenu(healScreen);
                break;
            case InteractionID.ENTRANCE_NPC3://Student
                ShowInfoDialog(Localization.GetText(12), Localization.GetText(14));
                break;
            case InteractionID.BASAR_NPC1:// Ninja
                ShowInfoDialog(Localization.GetText(13), Localization.GetText(31));
                break;
            case InteractionID.BASAR_NPC2:// potion dealer
                OpenMenu(potionScreen);
                break;
            case InteractionID.BASAR_NPC3:// food dealer
                OpenMenu(foodScreen);
                break;
            case InteractionID.HGATE_GATE_ENTRY:
                if (humangatewaystage.isFinished())
                    ShowInfoDialog(Localization.GetText(10), Localization.GetText(30));
                else
                {
                    StartCombat(humangatewaystage);
                }
                break;
            case InteractionID.HGATE_NPC1: // Guard
                ShowInfoDialog(Localization.GetText(40), Localization.GetText(34));
                break;
            case InteractionID.HGATE_NPC2:// Apprentice
                ShowInfoDialog(Localization.GetText(32), Localization.GetText(33));
                break;
            case InteractionID.MARKET_NPC1:// Apprentice
                ShowInfoDialog(Localization.GetText(32), Localization.GetText(28));
                break;
            case InteractionID.MARKET_NPC2:// merchant
                currentShopItems = shopItemsMarket;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.MARKET_NPC3: // herbs guy
                currentShopItems = herbs;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Drink);
                OpenMenu(herbguyScreen);
                break;
            case InteractionID.PUB_NPC1://Host
                if (pubhostDialogProgress == 0)
                {
                    ShowInfoDialog(Localization.GetText(20), Localization.GetText(21));
                    pubhostDialogProgress = 1;
                }
                else if (pubhostDialogProgress == 1)
                {
                    ShowInfoDialog(Localization.GetText(20), Localization.GetText(22));
                    pubhostDialogProgress = 2;
                }
                else if (pubhostDialogProgress == 2)
                {
                    ShowYesNoDialog(Localization.GetText(20), Localization.GetText(23), ResetSkills, CloseMenu);
                }
                break;
            case InteractionID.PUB_NPC2:// sitting drinker
                ShowInfoDialog(Localization.GetText(25), Localization.GetText(26));
                break;
            case InteractionID.PUB_NPC3:// gambler
                OpenMenu(gambleScreen);
                break;
            case InteractionID.PUB_NPC4:// Drinker with bottle
                ShowInfoDialog(Localization.GetText(25), Localization.GetText(27));
                break;
            case InteractionID.ARMORY_NPC1:// armor merchant
                currentShopItems = shopItemsArmory1;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.ARMORY_NPC2: // apprentice
                ShowInfoDialog(Localization.GetText(32), Localization.GetText(29));
                break;
            case InteractionID.ARMORY_NPC3:// weapon merchant
                currentShopItems = shopItemsArmory2;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.LIBRARY_NPC1:// medicine olle
                // Herbs in inventory?
                bool hasHerbs = false;
                bool hasBlackHerbs = false;
                for (int i = 0; i < inventoryItems.Length; i++)
                {
                    if (inventoryItems[i] != null)
                    {
                        if (inventoryItems[i].Equals(herbsItem))
                        {
                            hasHerbs = true;
                            inventoryItems[i] = medicineItem;
                        }
                        else if (inventoryItems[i].Equals(blackHerbsItem))
                        {
                            hasBlackHerbs = true;
                            inventoryItems[i] = specialMedicineItem;
                        }
                    }
                }
                if (hasBlackHerbs)
                    ShowInfoDialog(Localization.GetText(15), Localization.GetText(16));
                else if (hasHerbs)
                    ShowInfoDialog(Localization.GetText(15), Localization.GetText(17));
                else
                    ShowInfoDialog(Localization.GetText(15), Localization.GetText(18));
                break;
            case InteractionID.LIBRARY_NPC2: // Scribe
                ShowInfoDialog(Localization.GetText(13), Localization.GetText(35));
                break;
            case InteractionID.LIBRARY_NPC3: // relaxing ninja
                ShowInfoDialog(Localization.GetText(36), Localization.GetText(37));
                break;
            case InteractionID.LIBRARY_BOOKS:
                OpenMenu(bookshelfScreen);
                break;
            case InteractionID.SECRET:
                // Ring in inventory?
                for (int i = 0; i < inventoryItems.Length; i++)
                {
                    if (inventoryItems[i] != null
                        && inventoryItems[i].Equals(secretringItem))
                    {
                        inventoryItems[i] = null;
                        Skillpoints++;
                        ShowInfoDialog(Localization.GetText(10), Localization.GetText(51));
                        return;
                    }
                }
                break;
            case InteractionID.MGATE_GATE_ENTRY:
                if (monstergatewaystage.isFinished())
                    ShowInfoDialog(Localization.GetText(10), Localization.GetText(30));
                else
                {
                    StartCombat(monstergatewaystage);
                }
                break;
            case InteractionID.MGATE_NPC1:// merchant
                currentShopItems = shopItemsMGate;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.MGATE_NPC2://Guard
                ShowInfoDialog(Localization.GetText(40), Localization.GetText(43));
                break;
            case InteractionID.MGATE_NPC3://Dark Ninja
                ShowInfoDialog(Localization.GetText(44), Localization.GetText(45));
                break;
            case InteractionID.MGATE_NOTE:
                ShowInfoDialog(Localization.GetText(41), Localization.GetText(42));
                break;
            case InteractionID.TRAINING_ENTRY:
                OpenMenu(trainingScreen);
                break;
            case InteractionID.PREDARK_NPC1:
                // Ring already received or can added to inventory ?
                bool ringgiven = false;
                if (!ringreceived)
                    for (int i = 0; i < inventoryItems.Length; i++)
                    {
                        if (inventoryItems[i] == null)
                        {
                            inventoryItems[i] = secretringItem;
                            ringgiven = true;
                            ringreceived = true;
                            break;
                        }
                    }
                if (ringgiven)
                    ShowInfoDialog(Localization.GetText(47), Localization.GetText(48));
                else if (ringreceived)
                    ShowInfoDialog(Localization.GetText(47), Localization.GetText(49));
                else // No inventory space
                    ShowInfoDialog(Localization.GetText(47), Localization.GetText(50));
                break;
            case InteractionID.PREDARK_NPC2://Dark Ninja
                ShowInfoDialog(Localization.GetText(44), Localization.GetText(46));
                break;
            case InteractionID.DGATE_GUARD:
                ShowInfoDialog(Localization.GetText(40), Localization.GetText(54));
                break;
            case InteractionID.DGATE_NPC:
                ShowInfoDialog(Localization.GetText(52), Localization.GetText(53));
                break;
            case InteractionID.DGATE_MERCHANT:
                currentShopItems = shopItemsDGate;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.DGATE_MERCHANT2:
                currentShopItems = shopItemsNinja;
                acceptedItemTypes.Clear();
                acceptedItemTypes.Add(Item.Type.Weapon);
                acceptedItemTypes.Add(Item.Type.Shield);
                acceptedItemTypes.Add(Item.Type.Suit);
                acceptedItemTypes.Add(Item.Type.Head_Gear);
                OpenMenu(shopScreen);
                break;
            case InteractionID.DGATE_GATE_ENTRY:
                if (darkgatestage.isFinished())
                    ShowInfoDialog(Localization.GetText(10), Localization.GetText(30));
                else
                {
                    StartCombat(darkgatestage);
                }
                break;
            case InteractionID.END_EXIT:
                if (humangatewaystage.isFinished())
                {
                    GetComponent<AudioSource>().Stop();
                    OpenMenu(endScreen);
                }
                else
                {
                    ShowInfoDialog(Localization.GetText(10), Localization.GetText(11));
                }
                break;
        }
    }

    public void OpenMenu(GameObject menu)
    {
        if (currentMenu)
            currentMenu.SetActive(false); // deactivate previous menu
        currentMenu = menu;
        currentMenu.SetActive(true);
        bottomview.SetActive(false);
    }

    public static void OpenInventory()
    {
        if (instance)
            instance.OpenMenu(instance.inventoryScreen);
    }
    public static void OpenSkilltree()
    {
        if (instance)
            instance.OpenMenu(instance.skillsMenu);
    }

    public void ShowInfoDialog(string title, string message)
    {
        Debug.Log("ShowInfoDialog");
        OpenMenu(infoDialog);
        bottomview.SetActive(true);
        infoDialogTitle.text = title;
        infoDialogMessage.text = message;
    }
    public void ShowYesNoDialog(string title, string message, Action yesAction, Action noAction)
    {
        Debug.Log("ShowYesNoDialog");
        OpenMenu(yesnoDialog);
        bottomview.SetActive(true);
        yesnoDialogTitle.text = title;
        yesnoDialogMessage.text = message;
        dialogOptionYes = yesAction;
        dialogOptionNo = noAction;
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
    private void InitCharacter()
    {
        Debug.Log("InitCharacter Class: " + MyClass);
        foreach (Skill s in allSkills)
            s.currentlevel = 0;
        restsremaining = 10;
        humangatewaystage.currentLevel = 0;
        monstergatewaystage.currentLevel = 0;
        darkgatestage.currentLevel = 0;
        ringreceived = false;
        bookbonusreceived = false;
        pubhostDialogProgress = 0;
        Skillpoints = 0;
        Gold = 75;
        Level = 1;
        ExpNext = 50;
        Exp = 0;
        LifePotions = 5;
        ManaPotions = 5;
        MaxEng = 50;
        Eng = 50;
        currentRoom = FindRoomByID(RoomID.ENTRANCE);
        inventoryItems = new Item[8];
        playerchar.shield = null;
        playerchar.suit = null;
        playerchar.headgear = null;
        switch (MyClass)
        {
            case CharacterClass.Balanced:
                MaxLife = 100;
                Life = 100;
                MaxMana = 80;
                Mana = 80;
                Strength = 8;
                Dex = 4;
                Magic = 4;
                Weapon = FindItemByName("Iron Knife");
                FindSkillByName("Shurikens").currentlevel = 1;
                break;
            case CharacterClass.Warrior:
                MaxLife = 120;
                Life = 120;
                MaxMana = 40;
                Mana = 40;
                Strength = 12;
                Dex = 3;
                Magic = 2;
                Weapon = FindItemByName("Iron Knife");
                FindSkillByName("Stab").currentlevel = 1;
                break;
            case CharacterClass.Spellcaster:
                MaxLife = 60;
                Life = 60;
                MaxMana = 120;
                Mana = 120;
                Strength = 6;
                Dex = 3;
                Magic = 6;
                Weapon = FindItemByName("Energy Knife");
                FindSkillByName("Charge").currentlevel = 1;
                break;
            case CharacterClass.Ninja:
                MaxLife = 80;
                Life = 80;
                MaxMana = 80;
                Mana = 80;
                Strength = 8;
                Dex = 6;
                Magic = 3;
                Weapon = FindItemByName("Iron Knife");
                FindSkillByName("ShadowBlend").currentlevel = 1;
                break;
        }
        SaveAllPrefs();
    }

    // Global Functions

    // refs
    private static GameController instance;
    public static GameController Instance { get => instance; }
    private static InteractionID currentInteraction;
    private static Room currentRoom;
    private static bool shallInit = false, resourcesLoaded = false;
    private static Room[] allRooms;
    private static Stage humangatewaystage, monstergatewaystage, darkgatestage;
    private static event Action dialogOptionYes, dialogOptionNo;

    // stats
    private static Character playerchar;
    private static CharacterClass mClass;
    private static int skillpoints;

    // items
    private static Item[] allItems;
    private static ItemContainer herbs, shopItemsMarket, shopItemsArmory1, shopItemsArmory2, shopItemsMGate, shopItemsDGate, shopItemsNinja;
    private static Item[] inventoryItems = new Item[8];
    private static ItemContainer currentShopItems = null;
    private static List<Item.Type> acceptedItemTypes = new List<Item.Type>();

    // progress
    private static bool ringreceived = false, bookbonusreceived = false;
    private static Skill[] allSkills;
    private static int restsremaining;
    private static int prestigeLevel;
    private static int pubhostDialogProgress;

    public static Item.Type[] GetAcceptedItemTypes()
    {
        return acceptedItemTypes.ToArray();
    }

    public static int GetTotalInventorySpace()
    {
        return inventoryItems.Length;
    }

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
    public static Character PlayerChar { get => playerchar; }

    public static Item Weapon { get => playerchar.weapon; set => playerchar.weapon = value; }
    public static Item Shield { get => playerchar.shield; set => playerchar.shield = value; }
    public static Item Suit { get => playerchar.suit; set => playerchar.suit = value; }
    public static Item Headgear { get => playerchar.headgear; set => playerchar.headgear = value; }

    public static InteractionID CurrentInteraction
    {
        get => currentInteraction; set => currentInteraction = value;
    }

    public static Room FindRoomByID(RoomID id)
    {
        foreach (Room r in allRooms)
            if (r.id == id)
                return r;
        return null;
    }
    public static Skill FindSkillByName(string skillname)
    {
        foreach (Skill s in allSkills)
            if (s.name.Equals(skillname))
                return s;
        Debug.LogWarning("Cannot find Skill: " + skillname);
        return null;
    }
    public static Skill FindSkillByActionId(CombatAction action)
    {
        foreach (Skill s in allSkills)
            if (s.associatedAction == action)
                return s;
        Debug.LogWarning("Cannot find Skill: " + action);
        return null;
    }

    public static Item FindItemByName(string itemname)
    {
        foreach (Item i in allItems)
            if (i.name.Equals(itemname))
                return i;
        Debug.LogWarning("Cannot find Skill: " + itemname);
        return null;
    }
    public static Item FindItemByIndex(int idx)
    {
        if (idx >= 0 && idx < allItems.Length)
            return allItems[idx];

        return null;
    }
    public static int GetItemIndex(Item item)
    {
        if (item)
        {
            for (int i = 0; i < allItems.Length; i++)
                if (allItems[i].name.Equals(item.name))
                    return i;
            Debug.LogWarning("Item not found: " + item);
        }
        return -1;
    }

    public static bool TryEquip(Item item)
    {
        bool canputon = item.strengthRequired <= Strength;
        switch (item.type)
        {
            case Item.Type.Head_Gear:
                if (Headgear != null)
                    canputon = TryPutInventory(Headgear);
                if (canputon)
                    Headgear = item;
                return canputon;
            case Item.Type.Weapon:
                if (Weapon != null)
                    canputon = TryPutInventory(Weapon);
                if (canputon)
                    Weapon = item;
                return canputon;
            case Item.Type.Shield:
                if (Shield != null)
                    canputon = TryPutInventory(Shield);
                if (canputon)
                    Shield = item;
                return canputon;
            case Item.Type.Suit:
                if (Suit != null)
                    canputon = TryPutInventory(Suit);
                if (canputon)
                    Suit = item;
                return canputon;
        }
        return false;
    }

    public static bool TryPutInventory(Item item)
    {

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = item;
                return true;
            }
        }
        return false;
    }

    public static int PrestigeLevel()
    {
        return prestigeLevel;
    }

    public static void NextPrestigeLevel()
    {
        prestigeLevel++;
        restsremaining = 10;
        humangatewaystage.currentLevel = 0;
        monstergatewaystage.currentLevel = 0;
        darkgatestage.currentLevel = 0;
    }

    public static void ResetSkills()
    {
        int skillpointsSpent = 0;
        foreach (Skill s in allSkills)
        {
            skillpointsSpent += s.currentlevel;
            s.currentlevel = 0;
        }
        Skillpoints += skillpointsSpent;
        instance.ShowInfoDialog(Localization.GetText(10), Localization.GetText(24));
    }

    public static CharacterClass MyClass
    {
        get => mClass; set
        {
            mClass = value;
        }
    }

    public static string MyClassname
    {
        get
        {
            switch (mClass)
            {
                case CharacterClass.Balanced:
                    return Localization.GetText(489);
                case CharacterClass.Warrior:
                    return Localization.GetText(493);
                case CharacterClass.Spellcaster:
                    return Localization.GetText(491);
                case CharacterClass.Ninja:
                    return Localization.GetText(495);
                default:
                    return "INVALID";
            }
        }
    }

    public static int Gold
    {
        get => playerchar.gold; set
        {
            playerchar.gold = value;
        }
    }
    public static int Level
    {
        get => playerchar.level; set
        {
            playerchar.level = value;
        }
    }
    public static int Skillpoints
    {
        get => skillpoints; set
        {
            skillpoints = value;
        }
    }
    public static int LifePotions
    {
        get => playerchar.lifePotions; set
        {
            playerchar.lifePotions = value;
        }
    }
    public static int ManaPotions
    {
        get => playerchar.manaPotions; set
        {
            playerchar.manaPotions = value;
        }
    }
    public static int Strength
    {
        get => playerchar.strength; set
        {
            playerchar.strength = value;
        }
    }
    public static int Dex
    {
        get => playerchar.dex; set
        {
            playerchar.dex = value;
        }
    }
    public static int Magic
    {
        get => playerchar.magic; set
        {
            playerchar.magic = value;
        }
    }

    public static float Life
    {
        get => playerchar.life; set
        {
            playerchar.life = Mathf.Clamp(value, 0, playerchar.maxlife);
        }
    }
    public static float MaxLife
    {
        get => playerchar.maxlife; set
        {
            playerchar.maxlife = Mathf.Max(value, 1);
        }
    }
    public static float Mana
    {
        get => playerchar.mana; set
        {
            playerchar.mana = Mathf.Clamp(value, 0, playerchar.GetMaxMana());
        }
    }
    public static float MaxMana
    {
        get => playerchar.GetMaxMana(); set
        {
            playerchar.maxmana = Mathf.Max(value, 1);
        }
    }
    public static float Eng
    {
        get => playerchar.eng; set
        {
            playerchar.eng = Mathf.Clamp(value, 0, playerchar.maxeng);
        }
    }
    public static float MaxEng
    {
        get => playerchar.maxeng; set
        {
            playerchar.maxeng = Mathf.Max(value, 1);
        }
    }
    public static float Exp
    {
        get => playerchar.exp; set
        {
            playerchar.exp = value;
            if (playerchar.eng < 0)
                playerchar.eng = 0;
            while (playerchar.exp >= playerchar.expNext)
                LevelUp();
        }
    }
    public static float ExpNext
    {
        get => playerchar.expNext; set
        {
            playerchar.expNext = Mathf.Max(value, 1);
        }
    }

    public static int RestsRemaining { get => restsremaining; set => restsremaining = value; }

    public static int GetPhysDmg()
    {
        return playerchar.GetPhysDmg();
    }

    public static int GetPhysDef()
    {
        int def = playerchar.strength / 2;
        if (playerchar.weapon) def += playerchar.weapon.physDef;
        if (playerchar.suit) def += playerchar.suit.physDef;
        if (playerchar.headgear) def += playerchar.headgear.physDef;
        return def;
    }

    public static int GetMagicDmg()
    {
        return playerchar.GetMagicDmg();
    }

    public static int GetMagicDef()
    {
        int def = playerchar.magic / 2;
        if (playerchar.weapon) def += playerchar.weapon.magicDef;
        if (playerchar.suit) def += playerchar.suit.magicDef;
        if (playerchar.headgear) def += playerchar.headgear.magicDef;
        return def;
    }

    public static void InitCharacter(CharacterClass c)
    {
        MyClass = c;
        shallInit = true; // remember to init later when Start is called and instance is ready
    }
    public static int GetItemSellPrice(Item i)
    {
        if (i == null)
            return 0;
        return i.value / 2;
    }

    public static bool SellItem(Item i)
    {
        if (i == null)
            return false;
        if (!acceptedItemTypes.Contains(i.type))
            return false;
        Gold += GetItemSellPrice(i);
        return true;
    }

    public static bool UseLifePotion()
    {
        if (LifePotions <= 0)
            return false;
        Life += 80;
        LifePotions--;
        return true;
    }

    public static bool UseManaPotion()
    {
        if (ManaPotions <= 0)
            return false;
        Mana += 80;
        ManaPotions--;
        return true;
    }
    public static void OnYesClicked()
    {
        dialogOptionYes?.Invoke();
    }
    public static void OnNoClicked()
    {
        dialogOptionNo?.Invoke();
    }

    private static void LevelUp()
    {
        Level++;
        playerchar.exp -= playerchar.expNext;
        ExpNext += Level * 15;
        // common upgrades
        Skillpoints++;
        if (Level % 5 == 0)
            Skillpoints++; // Bonus point
        MaxLife += 5;
        MaxMana = playerchar.maxmana + 5;
        MaxEng += 3;
        // class specific upgrades
        switch (MyClass)
        {
            case CharacterClass.Balanced:
                MaxLife += 5;
                Strength++;
                Dex++;
                Magic++;
                break;
            case CharacterClass.Warrior:
                MaxLife += 10;
                Strength += 2;
                break;
            case CharacterClass.Spellcaster:
                MaxMana = playerchar.maxmana + 5;
                Magic += 2;
                Dex++;
                break;
            case CharacterClass.Ninja:
                MaxLife += 5;
                MaxMana = playerchar.maxmana + 5;
                Strength++;
                Dex += 2;
                break;
        }
        // refill all
        Eng = MaxEng;
        Life = MaxLife;
        Mana = MaxMana;
    }

    public static void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public static void LoadWorldScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCombatScene()
    {
        SceneManager.LoadScene(2);
    }

    private static void SwitchToCombatScene()
    {
        Instance.stageTitle.text = CombatController.CurrentStage.title;
        Instance.stageLevel.text = "Level " + (CombatController.CurrentStage.currentLevel + 1);
        Instance.combatIntroScreen.SetActive(true);
        Instance.Invoke(nameof(LoadCombatScene), 0.1f);
    }

    private static void StartCombat(Stage stage)
    {
        CombatController.InitCombat(stage);
        SwitchToCombatScene();
    }

    public static void StartTraining(int level)
    {
        CombatController.InitTraining(level);
        SwitchToCombatScene();
    }

    public static void CloseMenu()
    {
        if (instance && instance.currentMenu)
        {
            instance.currentMenu.SetActive(false);
            instance.currentMenu = null;
            instance.bottomview.SetActive(true);
        }
        currentShopItems = null;
        dialogOptionYes = null;
        dialogOptionNo = null;
    }

    public static void OnAllBooksRead()
    {
        if (!bookbonusreceived)
        {
            bookbonusreceived = true;
            Skillpoints++;
            Instance.ShowInfoDialog(Localization.GetText(10), "Reading all the books grants you great knowledge in form of a bonus Skillpoint.");
        }
    }

    private static void LoadResources()
    {
        Debug.Log("LoadResources()");

        humangatewaystage = Resources.Load<Stage>("Stages/Human");
        monstergatewaystage = Resources.Load<Stage>("Stages/Monster");
        darkgatestage = Resources.Load<Stage>("Stages/Dark");
        allSkills = Resources.LoadAll<Skill>("Skills");
        allRooms = Resources.LoadAll<Room>("Rooms");
        allItems = Resources.LoadAll<Item>("Items");
        herbs = Resources.Load<ItemContainer>("Items/Container/Herbs");
        shopItemsMarket = Resources.Load<ItemContainer>("Items/Container/shopItemsMarket");
        shopItemsArmory1 = Resources.Load<ItemContainer>("Items/Container/shopItemsArmory1");
        shopItemsArmory2 = Resources.Load<ItemContainer>("Items/Container/shopItemsArmory2");
        shopItemsMGate = Resources.Load<ItemContainer>("Items/Container/shopItemsMGate");
        shopItemsDGate = Resources.Load<ItemContainer>("Items/Container/shopItemsDGate");
        shopItemsNinja = Resources.Load<ItemContainer>("Items/Container/shopItemsNinja");

        playerchar = Resources.Load<Character>("Character/Player");


        resourcesLoaded = true;
    }

    public static void LoadAllPrefs()
    {
        Debug.Log("LoadAllPrefs()");
        if (!resourcesLoaded)
        {
            LoadResources();
        }
        MyClass = (CharacterClass)(PlayerPrefs.GetInt("class", (int)CharacterClass.Invalid));
        Gold = PlayerPrefs.GetInt("gold", 0);
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
        Skillpoints = PlayerPrefs.GetInt("skillpoints", 0);
        int roomid = PlayerPrefs.GetInt("room", (int)(RoomID.ENTRANCE));
        currentRoom = FindRoomByID((RoomID)roomid);
        Weapon = FindItemByIndex(PlayerPrefs.GetInt("weapon", -1));
        Shield = FindItemByIndex(PlayerPrefs.GetInt("shield", -1));
        Headgear = FindItemByIndex(PlayerPrefs.GetInt("headgear", -1));
        Suit = FindItemByIndex(PlayerPrefs.GetInt("suit", -1));
        for (int i = 0; i < inventoryItems.Length; i++)
            SetInventoryItem(i, FindItemByIndex(PlayerPrefs.GetInt("item_" + i, -1)));
        for (int i = 0; i < allSkills.Length; i++)
            allSkills[i].currentlevel = PlayerPrefs.GetInt("skill_" + i, 0);
        ringreceived = PlayerPrefs.GetInt("ringreceived", 0) > 0;
        bookbonusreceived = PlayerPrefs.GetInt("bookbonusreceived", 0) > 0;
        restsremaining = PlayerPrefs.GetInt("restsremaining", 10);
        humangatewaystage.currentLevel = PlayerPrefs.GetInt("humangatewaystageLevel", 0);
        monstergatewaystage.currentLevel = PlayerPrefs.GetInt("monstergatewaystageLevel", 0);
        darkgatestage.currentLevel = PlayerPrefs.GetInt("darkgatestageLevel", 0);
        prestigeLevel = PlayerPrefs.GetInt("prestigeLevel", 0);
        pubhostDialogProgress = PlayerPrefs.GetInt("pubhostDialogProgress", 0);
    }

    public static void SaveAllPrefs()
    {
        Debug.Log("SaveAllPrefs()");
        PlayerPrefs.SetInt("class", (int)MyClass);
        PlayerPrefs.SetInt("gold", Gold);
        PlayerPrefs.SetInt("level", Level);
        PlayerPrefs.SetInt("maxlife", (int)MaxLife);
        PlayerPrefs.SetInt("life", (int)Life);
        PlayerPrefs.SetInt("maxmana", (int)playerchar.maxmana);
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
        PlayerPrefs.SetInt("skillpoints", Skillpoints);
        if (currentRoom)
            PlayerPrefs.SetInt("room", (int)currentRoom.id);
        else
            PlayerPrefs.SetInt("room", (int)RoomID.ENTRANCE);

        PlayerPrefs.SetInt("weapon", GetItemIndex(Weapon));
        PlayerPrefs.SetInt("shield", GetItemIndex(Shield));
        PlayerPrefs.SetInt("headgear", GetItemIndex(Headgear));
        PlayerPrefs.SetInt("suit", GetItemIndex(Suit));
        for (int i = 0; i < inventoryItems.Length; i++)
            PlayerPrefs.SetInt("item_" + i, GetItemIndex(inventoryItems[i]));
        for (int i = 0; i < allSkills.Length; i++)
            PlayerPrefs.SetInt("skill_" + i, allSkills[i].currentlevel);
        PlayerPrefs.SetInt("ringreceived", ringreceived ? 1 : 0);
        PlayerPrefs.SetInt("bookbonusreceived", bookbonusreceived ? 1 : 0);
        PlayerPrefs.SetInt("restsremaining", restsremaining);
        PlayerPrefs.SetInt("humangatewaystageLevel", humangatewaystage.currentLevel);
        PlayerPrefs.SetInt("monstergatewaystageLevel", monstergatewaystage.currentLevel);
        PlayerPrefs.SetInt("darkgatestageLevel", darkgatestage.currentLevel);
        PlayerPrefs.SetInt("prestigeLevel", prestigeLevel);
        PlayerPrefs.SetInt("pubhostDialogProgress", pubhostDialogProgress);
    }
}
