using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : UIBase
{
    public static Shop Instance { get; private set; }

    [SerializeField] private GameObject shopUI;

    public ShopInfoSO shopInfo;

    [SerializeField] private Transform itemContent;
    [SerializeField] private GameObject uiElement;

    private int activeSlot = 0;

    [Header("ShopCountWindow")]
    [SerializeField] private GameObject shopItemMenuUI;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Slider countItemSlider;
    [SerializeField] private TextMeshProUGUI countItemText;
    private int countItemToShop = 1;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        StartCoroutine(UpdateUI());
    }

    private void Update()
    {
        if (shopItemMenuUI.activeSelf)
        {
            List<InventorySlot> items = Inventory.Instance.items;
            InventorySlot findGold = items.Find(gold => gold.item.itemName == "Gold");

            countItemSlider.minValue = 0;

            if (shopInfo.shopSlots[activeSlot].price == 0)
                countItemSlider.maxValue = 999;
            else
            {
                if (findGold != null)
                    if (shopInfo.shopSlots[activeSlot].price == 0)
                        countItemSlider.maxValue = 999;
                    else
                        countItemSlider.maxValue = findGold.count / shopInfo.shopSlots[activeSlot].price;
                else countItemSlider.maxValue = 0;
            }

            itemName.text = shopInfo.shopSlots[activeSlot].itemInfo.item.itemName;
            countItemToShop = (int)countItemSlider.value;
            countItemText.text = countItemSlider.value.ToString();
        }
    }

    private IEnumerator UpdateUI()
    {
        DestroyUIElements(itemContent);
        CreateUIElement(itemContent, uiElement, shopInfo.shopSlots);

        yield return null;

        UpdateUIElementNames();
    }

    private void UpdateUIElementNames()
    {
        for (int i = 0; i < shopInfo.shopSlots.Count; i++)
        {
            itemContent.GetChild(i).GetComponent<Image>().sprite = shopInfo.shopSlots[i].itemInfo.item.itemSprite;
        }
    }

    public override void GetItemInfo()
    {
        activeSlot = GetClickedButton() - 1;

        shopItemMenuUI.SetActive(true);
    }

    public void BuyItem()
    {
        SoundSystem.Instance.PlaySound("BuySellItem");

        List<InventorySlot> items = Inventory.Instance.items;

        InventorySlot findGold = items.Find(gold => gold.item.itemName == "Gold");
        
        InventorySlot product = shopInfo.shopSlots[activeSlot].itemInfo;
        int productPrice = shopInfo.shopSlots[activeSlot].price;

        if (findGold != null && findGold.count >= productPrice && countItemToShop > 0)
        {
            Inventory.Instance.RemoveItem(findGold, productPrice * countItemToShop);
            product.count = countItemToShop;
            Inventory.Instance.AddItem(product);
        }

        countItemSlider.value = 0;

        shopItemMenuUI.SetActive(false);
    }
}
