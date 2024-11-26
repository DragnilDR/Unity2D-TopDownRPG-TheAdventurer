using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private SpriteRenderer chestSprite;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;

    [SerializeField] private List<InventorySlot> itemInChest = new();
    [SerializeField] private List<LootInChestSO> possibleItems = new();

    private GameObject chestUI;
    private Transform closeButton;

    [SerializeField] private bool playerIsFound = false;

    private void Start()
    {
        chestSprite = GetComponent<SpriteRenderer>();

        GenerateLoot();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerIsFound)
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsFound = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsFound = false;
        }
    }

    public void GenerateLoot()
    {
        int randomNumber = Random.Range(0, possibleItems.Count);

        itemInChest = possibleItems[randomNumber].possibleItems.ConvertAll(item => new InventorySlot() { item = item.item, count = item.count });
    }


    public async void OpenChest()
    {
        SoundSystem.Instance.PlaySound("ChestOpen");

        chestUI = UIManager.Instance.chestUI;

        closeButton = chestUI.transform.Find("CloseButton");
        closeButton.GetComponent<Button>().onClick.AddListener(() => CloseChest());

        var chest = chestUI.GetComponent<InventoryUI>();
        chest.itemList = itemInChest;

        PlayerCamera.Instance.movingLock = true;

        chestSprite.sprite = openSprite;

        await Task.Delay(300);
        
        chestUI.SetActive(true);
    }

    public void CloseChest()
    {
        SoundSystem.Instance.PlaySound("ChestClose");

        chestSprite.sprite = closeSprite;

        chestUI.SetActive(false);

        PlayerCamera.Instance.movingLock = false;
    }
}
