using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private List<ItemInfoSO> itemList = new();

    private ItemInfoSO GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101);
        List<ItemInfoSO> possibleItems = new();

        foreach (ItemInfoSO item in itemList)
        {
            if (randomNumber <= item.dropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            ItemInfoSO droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;
    }

    public void InstantiateItem(Vector3 spawnPos)
    {
        ItemInfoSO droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            GameObject itemGameObject = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            if (itemGameObject != null)
            {
                itemGameObject.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);

                itemGameObject.GetComponent<ItemPickup>().inventorySlot.item = droppedItem;
                itemGameObject.GetComponent<ItemPickup>().inventorySlot.count = Random.Range(1, 6);
                itemGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.itemSprite;
                itemGameObject.name = droppedItem.itemName;
            }
        }
    }
}
