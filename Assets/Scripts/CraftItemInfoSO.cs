using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Craft", menuName = "Create Craft/New Craft")]
public class CraftItemInfoSO : ScriptableObject
{
    public List<InventorySlot> neededItemForCraft = new();
    public InventorySlot resultItem;
}
