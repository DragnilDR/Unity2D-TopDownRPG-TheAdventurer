using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestBoardUI : UIBase
{
    public static QuestBoardUI Instance { get; private set; }

    private QuestManager questManager;

    [SerializeField] private Transform itemContent;
    [SerializeField] private GameObject questItemUI;

    [SerializeField] private GameObject questInfoUI;

    public int activeSlot = 0;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        questManager = FindObjectOfType<QuestManager>();
        DestroyUIElements(itemContent);
        CreateUIElement(itemContent, questItemUI, questManager.questSlot);
    }

    public override void GetItemInfo()
    {
        SoundSystem.Instance.PlaySound("TurnPage");

        activeSlot = GetClickedButton() - 1;

        questInfoUI.SetActive(true);

        gameObject.SetActive(false);
    }
}
