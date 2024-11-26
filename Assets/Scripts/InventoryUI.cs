using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class InventoryUI : UIBase
{
    private Player player;

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private Transform itemContent;
    [SerializeField] private GameObject uiElement;

    [SerializeField] private TextMeshProUGUI itemNameUI;
    [SerializeField] private TextMeshProUGUI itemDescriptionUI;

    private int activeSlot = 0;

    [Header("Window")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Slider countItemSlider;
    [SerializeField] private TextMeshProUGUI countItemText;
    private int countItem = 1;

    public List<InventorySlot> itemList = new();

    private void Start()
    {
        player = FindAnyObjectByType<Player>().GetComponent<Player>();
    }

    private void OnEnable()
    {
        activeSlot = 0;

        StartCoroutine(UpdateUI());
    }

    private void Update()
    {
        if (menuUI.activeSelf)
        {
            if (itemList.Count > 0)
            {
                countItemSlider.minValue = 1;
                countItemSlider.maxValue = itemList[activeSlot].count;
                countItem = (int)countItemSlider.value;
                countItemText.text = countItemSlider.value.ToString();
            }
            else if (itemList.Count <= 0)
                menuUI.SetActive(false);
        }
    }

    private IEnumerator UpdateUI()
    {
        DestroyUIElements(itemContent);
        CreateUIElement(itemContent, uiElement, Inventory.Instance.items);

        yield return null;

        UpdateUIElementNames();
        ItemInfoUI();
    }

    private void UpdateUIElementNames()
    {
        List<InventorySlot> itemList = Inventory.Instance.items;

        for (int i = 0; i < itemList.Count; i++)
        {
            var textComponent = itemContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();

            textComponent.text = itemList[i].item.itemName;
        }
    }

    private void ItemInfoUI()
    {
        if (itemList.Count == 0)
        {
            itemNameUI.text = " ";
            itemDescriptionUI.text = " ";
        }
        else
        {
            itemNameUI.text = $"{itemList[activeSlot].item.itemName} x{itemList[activeSlot].count}";
            itemDescriptionUI.text = itemList[activeSlot].item.itemDescription;

            if (itemList[activeSlot].isEquiped)
                itemNameUI.text = $"{itemList[activeSlot].item.itemName} x{itemList[activeSlot].count} Equiped";
        }
    }

    public override void GetItemInfo()
    {
        activeSlot = GetClickedButton() - 1;

        ItemInfoUI();
    }

    public void UseItem()
    {
        List<InventorySlot> itemList = Inventory.Instance.items;
        if (itemList.Count > 0)
        {
            switch (itemList[activeSlot].item.itemType)
            {
                case ItemInfoSO.ItemType.Consumables:

                    if (itemList[activeSlot].item.consumablesType == ItemInfoSO.ConsumablesType.HealthPotion)
                        if (player.stats.Health != 100)
                        {
                            player.stats.Health += itemList[activeSlot].item.plus;

                            if (player.stats.Health > 100)
                                player.stats.Health = 100;

                            var item = itemList[activeSlot];

                            Inventory.Instance.RemoveItem(itemList[activeSlot], 1);

                            if (item.count == 0)
                                activeSlot = 0;
                        }
                    break;
                case ItemInfoSO.ItemType.Equipment:
                    if (itemList[activeSlot].isEquiped)
                    {
                        DeEquipItem(itemList[activeSlot]);
                    }
                    else
                    {
                        EquipItem(itemList[activeSlot]);
                    }
                    break;
                case ItemInfoSO.ItemType.Item:
                    break;
                default:
                    break;
            }

            StartCoroutine(UpdateUI());
        }
    }

    private void EquipItem(InventorySlot item)
    {
        foreach (var i in Inventory.Instance.items)
        {
            if (i.item.equipmentType == item.item.equipmentType && i.isEquiped)
            {
                DeEquipItem(i);
            }
        }

        if (item.item.equipmentType == ItemInfoSO.EquipmentType.Weapon)
        {
            switch (item.item.attackType)
            {
                case ItemInfoSO.AttackType.Melle:
                    player.attackType = Player.AttackType.Melle;
                    break;
                case ItemInfoSO.AttackType.Range:
                    player.attackType = Player.AttackType.Range;
                    player.projectile = item.item.projectile;
                    break;
            }
        }

        itemList[activeSlot].isEquiped = true;

        player.stats += itemList[activeSlot].item.stats;

        SoundSystem.Instance.PlaySound("EquipItem");
    }

    private void DeEquipItem(InventorySlot item)
    {
        item.isEquiped = false;

        player.stats -= item.item.stats;

        if (item.item.equipmentType == ItemInfoSO.EquipmentType.Weapon)
        {
            player.attackType = Player.AttackType.Melle;
        }

        SoundSystem.Instance.PlaySound("DeEquipItem");
    }

    public void OpenMenu()
    {
        if (itemList.Count > 0)
            menuUI.SetActive(true);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }

    public void DropItem()
    {
        SoundSystem.Instance.PlaySound("DropItem");

        if (itemList.Count > 0)
        {
            GameObject droppedItem = Instantiate(itemPrefab, FindAnyObjectByType<Player>().transform.position, Quaternion.identity);
            droppedItem.name = itemList[activeSlot].item.itemName;
            droppedItem.GetComponent<ItemPickup>().inventorySlot.item = itemList[activeSlot].item;
            droppedItem.GetComponent<ItemPickup>().inventorySlot.count = countItem;

            Inventory.Instance.RemoveItem(itemList[activeSlot], countItem);

            activeSlot = 0;

            StartCoroutine(UpdateUI());
        }

        countItemSlider.value = 0;

        menuUI.SetActive(false);
    }

    public void TakeItem()
    {
        SoundSystem.Instance.PlaySound("TakeItem");

        if (itemList.Count > 0)
        {
            Inventory.Instance.AddItem(itemList[activeSlot]);

            if (itemList[activeSlot].count >= 1)
            {
                itemList[activeSlot].count -= countItem;
            }

            if (itemList[activeSlot].count <= 0)
            {
                itemList.Remove(itemList[activeSlot]);

                activeSlot = 0;
            }
        }

        StartCoroutine(UpdateUI());

        countItemSlider.value = 0;

        menuUI.SetActive(false);
    }
}
