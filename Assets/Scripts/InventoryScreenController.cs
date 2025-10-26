using UnityEngine;
using UnityEngine.UI;

public class InventoryScreenController : DefaultMenuButtonHandler
{
    public ItemSlotController[] inventorySlots;
    public ItemSlotController[] shopSlots;
    public ItemSlotController weaponslot, headgearslot, suitslot, shieldslot;
    public ItemSlotController pickedItemSlot;
    public ItemDescriptionPanelController itemDescription;
    public Text yourgold, yourstrength, sellvalue;
    public Text acceptedItems;

    private Item picketItem = null;

    private void Start()
    {
        weaponslot.OnSelectItem += SetSelectedItem;
        shieldslot.OnSelectItem += SetSelectedItem;
        headgearslot.OnSelectItem += SetSelectedItem;
        suitslot.OnSelectItem += SetSelectedItem;
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].OnSelectItem += SetSelectedItem;
        for (int i = 0; i < shopSlots.Length; i++)
            shopSlots[i].OnSelectItem += SetSelectedItem;
    }

    void OnEnable()
    {
        Sync();
    }

    public void SetSelectedItem(Item item)
    {
        if (itemDescription)
            itemDescription.DisplayedItem = item;
    }

    void Sync()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].Item = GameController.GetInventoryItem(i);
        for (int i = 0; i < shopSlots.Length; i++)
        {
            Item item = GameController.GetShopItem(i);
            //Debug.Log("ShopItem" + i + ": " + (item==null?"none":item.displayname));
            shopSlots[i].Item = item;
        }
        weaponslot.Item = GameController.Weapon;
        headgearslot.Item = GameController.Headgear;
        suitslot.Item = GameController.Suit;
        shieldslot.Item = GameController.Shield;
        pickedItemSlot.Item = picketItem;
        if (sellvalue)
        {
            sellvalue.text = "";
            if (picketItem)
                sellvalue.text = GameController.GetItemSellPrice(picketItem).ToString();
        }
        if(acceptedItems)
        {
            string txt = "";
            foreach (Item.Type t in GameController.GetAcceptedItemTypes())
                txt = txt + t.ToString() + ", ";
            txt.Trim(',');
            acceptedItems.text = txt;
        }
        if (yourgold)
        {
            yourgold.text = GameController.Gold.ToString();
        }
        if (yourstrength)
        {
            yourstrength.text = GameController.Strength.ToString();
        }
    }

    public override void Kreis()
    {
        Debug.Log("InventoryScreenController.Kreis()");
        if(picketItem == null)  // prevent losing the item in hand
            GameController.CloseMenu();
    }
    public override void L1()
    {
        Debug.Log("InventoryScreenController.L1()");
        if (picketItem == null)  // prevent losing the item in hand
            GameController.OpenSkilltree();
    }
    public override void R1()
    {
        Debug.Log("InventoryScreenController.R1()");
        if (picketItem == null)  // prevent losing the item in hand
            GameController.OpenSkilltree();
    }

    public void OnInventoryslotClicked(int idx)
    {
        Item otheritem = GameController.GetInventoryItem(idx);
        GameController.SetInventoryItem(idx, picketItem);
        SetSelectedItem(picketItem == null ? otheritem : picketItem);
        picketItem = otheritem;
        Sync();
    }
    public void OnShopslotClicked(int idx)
    {
        if(picketItem == null)
        {
            Item shopitem = GameController.GetShopItem(idx);
            if(shopitem != null && shopitem.value <= GameController.Gold)
            {
                GameController.Gold -= shopitem.value;
                picketItem = shopitem;
                Sync();
            }
        }
    }

    public void OnWeaponslotClicked()
    {
        if (picketItem == null || // no item picked
            ((picketItem.type == Item.Type.Weapon) && (picketItem.strengthRequired <= GameController.Strength) )) // can equip picket item
        {
            Item otheritem = GameController.Weapon;
            GameController.Weapon = picketItem;
            SetSelectedItem(picketItem == null ? otheritem : picketItem);
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnShieldslotClicked()
    {
        if (picketItem == null || // no item picked
            ((picketItem.type == Item.Type.Shield) && (picketItem.strengthRequired <= GameController.Strength))) // can equip picket item
        {
            Item otheritem = GameController.Shield;
            GameController.Shield = picketItem;
            SetSelectedItem(picketItem == null ? otheritem : picketItem);
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnHeadgearslotClicked()
    {
        if (picketItem == null || // no item picked
            ((picketItem.type == Item.Type.Head_Gear) && (picketItem.strengthRequired <= GameController.Strength))) // can equip picket item
        {
            Item otheritem = GameController.Headgear;
            GameController.Headgear = picketItem;
            SetSelectedItem(picketItem == null ? otheritem : picketItem);
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnSuitslotClicked()
    {
        if (picketItem == null || // no item picked
            ((picketItem.type == Item.Type.Suit) && (picketItem.strengthRequired <= GameController.Strength))) // can equip picket item
        {
            Item otheritem = GameController.Suit;
            GameController.Suit = picketItem;
            SetSelectedItem(picketItem == null ? otheritem : picketItem);
            picketItem = otheritem;
            Sync();
        }
    }

    public void OnItemBinClicked()
    {
        picketItem = null;
        Sync();
    }

    public void OnSellClicked()
    {
        if (GameController.SellItem(picketItem))
        {
            picketItem = null;
            Sync();
        }
    }
}
