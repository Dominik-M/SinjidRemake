using UnityEngine;

public class InventoryScreenController : DefaultMenuButtonHandler
{
    public ItemSlotController[] inventorySlots;
    public ItemSlotController[] shopSlots;
    public ItemSlotController weaponslot, headgearslot, suitslot, shieldslot;
    public ItemSlotController pickedItemSlot;

    private Item picketItem = null;
    void OnEnable()
    {
        Sync();
    }

    void Sync()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].Item = GameController.GetInventoryItem(i);
        for (int i = 0; i < shopSlots.Length; i++)
            shopSlots[i].Item = GameController.GetShopItem(i);
        weaponslot.Item = GameController.Weapon;
        headgearslot.Item = GameController.Headgear;
        suitslot.Item = GameController.Suit;
        shieldslot.Item = GameController.Shield;
        pickedItemSlot.Item = picketItem;
    }
    public override void Kreis()
    {
        Debug.Log("InventoryScreenController.Kreis()");
        if(picketItem == null)  // prevent losing the item in hand
            GameController.CloseMenu();
    }

    public void OnInventoryslotClicked(int idx)
    {
        Item otheritem = GameController.GetInventoryItem(idx);
        GameController.SetInventoryItem(idx, picketItem);
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
        if (picketItem == null || picketItem.type == Item.Type.Weapon)
        {
            Item otheritem = GameController.Weapon;
            GameController.Weapon = picketItem;
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnShieldslotClicked()
    {
        if (picketItem == null || picketItem.type == Item.Type.Shield)
        {
            Item otheritem = GameController.Weapon;
            GameController.Shield = picketItem;
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnHeadgearslotClicked()
    {
        if (picketItem == null || picketItem.type == Item.Type.Head_Gear)
        {
            Item otheritem = GameController.Weapon;
            GameController.Headgear = picketItem;
            picketItem = otheritem;
            Sync();
        }
    }
    public void OnSuitslotClicked()
    {
        if (picketItem == null || picketItem.type == Item.Type.Suit)
        {
            Item otheritem = GameController.Weapon;
            GameController.Suit = picketItem;
            picketItem = otheritem;
            Sync();
        }
    }
}
