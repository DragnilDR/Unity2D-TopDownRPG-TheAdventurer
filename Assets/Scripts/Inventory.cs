using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemInfoSO item;
    public int count;

    public bool isEquiped;
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<InventorySlot> items = new();

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventorySlot itemSlotToAdd)
    {
        InventorySlot findItemInInventory = items.Find(item => item.item == itemSlotToAdd.item);

        if (findItemInInventory != null)
            findItemInInventory.count += itemSlotToAdd.count;
        else
        {
            InventorySlot inventorySlot = new()
            {
                item = itemSlotToAdd.item,
                count = itemSlotToAdd.count
            };

            items.Add(inventorySlot);
        }
    }

    public void RemoveItem(InventorySlot itemSlotToRemove, int itemCount)
    {
        if (itemSlotToRemove.count >= 1)
        {
            itemSlotToRemove.count -= itemCount;
        }

        if (itemSlotToRemove.count <= 0)
        {
            if (itemSlotToRemove.isEquiped == true)
                itemSlotToRemove.isEquiped = false;

            items.Remove(itemSlotToRemove);
        }
    }
}
