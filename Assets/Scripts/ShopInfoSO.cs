using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    public InventorySlot itemInfo;

    public int price;
}

[CreateAssetMenu(fileName = "ShopInfo", menuName = "CreateShopInfo/ShopInfo")]
public class ShopInfoSO : ScriptableObject
{
    public List<ShopSlot> shopSlots = new();
}
