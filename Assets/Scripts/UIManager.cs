using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Player player;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TextMeshProUGUI healthUI;

    [Header("Dialogue Window")]
    public GameObject dialogueWindow;

    [Header("Inventory")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject dropWindowUI;

    [Header("Chest")]
    public GameObject chestUI;

    [Header("Journal")]
    [SerializeField] private GameObject journalUI;

    [Header("Quest")]
    [SerializeField] private GameObject questBoardUI;
    [SerializeField] private GameObject questInfoUI;
    public GameObject passQuestUI;

    [Header("Shop")]
    public GameObject shopUI;
    [SerializeField] private GameObject shopItemUI;

    [SerializeField] private GameObject craftUI;

    [Header("Triggers")]
    [SerializeField] private Collider2D questBoardTrigger;
    [SerializeField] private Collider2D passQuestTrigger;
    [SerializeField] private Collider2D shopTrigger;

    private void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }

        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        if (player != null)
        {
            healthBar.fillAmount = player.stats.Health / 100f;
            staminaBar.fillAmount = player.stats.Stamina / 100f;

            healthUI.text = player.stats.Health.ToString();
        }
        else
        {
            player = FindAnyObjectByType<Player>();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider2D collider = player.GetComponent<Collider2D>();

            if (questBoardTrigger != null && questBoardTrigger.bounds.Intersects(collider.bounds))
            {
                ShowUI(questBoardUI);
            }
                
            if (passQuestTrigger != null && passQuestTrigger.bounds.Intersects(collider.bounds))
            {
                var dialogue = passQuestTrigger.GetComponent<DialogueRepository>();

                dialogueWindow.GetComponent<DialogueUI>().dialogueRepository = dialogue;

                ShowUI(dialogueWindow);
            }
                
            if (shopTrigger != null && shopTrigger.bounds.Intersects(collider.bounds))
            {
                var dialogue = shopTrigger.GetComponent<DialogueRepository>();

                dialogueWindow.GetComponent<DialogueUI>().dialogueRepository = dialogue;

                ShowUI(dialogueWindow);
            }
        }

        if (Menu.Instance.pauseGame)
        {
            HideAllUI();
        }

        if (Input.GetKeyDown(KeyCode.J) && !Menu.Instance.pauseGame)
        {
            SoundSystem.Instance.PlaySound("TurnPage");

            ShowUI(journalUI);
        }

        if (Input.GetKeyDown(KeyCode.I) && !Menu.Instance.pauseGame)
        {
            SoundSystem.Instance.PlaySound("TurnPage");

            var inventory = inventoryUI.GetComponent<InventoryUI>();
            inventory.itemList = Inventory.Instance.items;

            ShowUI(inventoryUI);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !Menu.Instance.pauseGame)
        {
            SoundSystem.Instance.PlaySound("TurnPage");

            ShowUI(craftUI);
        }
    }

    public void ShowUI(GameObject UIElement)
    {
        HideAllUI();
        UIElement.SetActive(true);
        PlayerCamera.Instance.movingLock = true;
    }

    public void HideAllUI()
    {
        PlayerCamera.Instance.movingLock = false;

        inventoryUI.SetActive(false);
        dropWindowUI.SetActive(false);

        chestUI.SetActive(false);

        journalUI.SetActive(false);

        questBoardUI.SetActive(false);
        questInfoUI.SetActive(false);
        passQuestUI.SetActive(false);

        shopUI.SetActive(false);
        shopItemUI.SetActive(false);

        craftUI.SetActive(false);
    }
}
