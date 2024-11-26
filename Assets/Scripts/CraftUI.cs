using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftUI : UIBase
{
    private List<CraftItemInfoSO> allCrafts;

    [SerializeField] private Transform craftContent;
    [SerializeField] private GameObject uiElement;
    [SerializeField] private Transform craftInfoContent;
    [SerializeField] private GameObject craftUIItem;
    [SerializeField] private Image resultCraftSpriteUI;
    [SerializeField] private TextMeshProUGUI resultCrafCountUI;

    [SerializeField] private TextMeshProUGUI craftNameUI;

    private int activeSlot = 0;

    private void Start()
    {
        UpdateAllCrafts();

        StartCoroutine(UpdateUI());
    }

    private void UpdateAllCrafts()
    {
        allCrafts = new(Resources.LoadAll<CraftItemInfoSO>("CraftInfo"));
    }

    private IEnumerator UpdateUI()
    {
        DestroyUIElements(craftContent);
        CreateUIElement(craftContent, uiElement, allCrafts);

        yield return null;

        UpdateUIElementNames();
        CraftInfoUI();
    }

    private void UpdateUIElementNames()
    {
        for (int i = 0; i < allCrafts.Count; i++)
        {
            var textComponent = craftContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();

            textComponent.text = allCrafts[i].resultItem.item.itemName;
        }
    }

    private void CraftInfoUI()
    {
        if (allCrafts.Count == 0)
        {
            craftNameUI.text = " ";
            resultCrafCountUI.text = $" ";
            resultCraftSpriteUI.sprite = null;
        }
        else
        {
            craftNameUI.text = $"{allCrafts[activeSlot].resultItem.item.name}";

            foreach (Transform craftUI in craftInfoContent)
            {
                Destroy(craftUI.gameObject);
            }

            foreach (var item in allCrafts[activeSlot].neededItemForCraft)
            {
                GameObject craftUIElement = Instantiate(craftUIItem, craftInfoContent);

                var craftCount = craftUIElement.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
                var craftSprite = craftUIElement.GetComponent<Image>();

                craftCount.text = $"x{item.count}";
                craftSprite.sprite = item.item.itemSprite;
            }

            resultCrafCountUI.text = $"x{allCrafts[activeSlot].resultItem.count}";
            resultCraftSpriteUI.sprite = allCrafts[activeSlot].resultItem.item.itemSprite;
        }
    }

    public override void GetItemInfo()
    {
        activeSlot = GetClickedButton() - 1;

        CraftInfoUI();
    }

    public void CraftItem()
    {
        List<InventorySlot> items = Inventory.Instance.items;
        CraftItemInfoSO resultItem = allCrafts[activeSlot];

        foreach (var craftItem in allCrafts[activeSlot].neededItemForCraft)
        {
            InventorySlot findItem = items.Find(item => item.item == craftItem.item);

            if (findItem != null)
            {
                if (findItem.Equals(null) || findItem.count < craftItem.count)
                {
                    return;
                }
            }
        }

        foreach (var craftItem in allCrafts[activeSlot].neededItemForCraft)
        {
            InventorySlot findItem = items.Find(item => item.item == craftItem.item);

            Inventory.Instance.RemoveItem(findItem, craftItem.count);
        }

        Inventory.Instance.AddItem(resultItem.resultItem);
    }
}
