using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsInChestInfo", menuName = "CreateItemItemsInChestInfo/Loot")]
public class LootInChestSO : ScriptableObject
{
    public List<InventorySlot> possibleItems = new();
}
