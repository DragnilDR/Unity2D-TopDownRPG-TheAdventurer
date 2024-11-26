using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class JournalUI : UIBase
{
    [SerializeField] private Transform questContent;
    [SerializeField] private GameObject uiElement;

    [SerializeField] private TextMeshProUGUI questNameUI;
    [SerializeField] private TextMeshProUGUI questDescriptionUI;

    [SerializeField] private TextMeshProUGUI questUI;

    private int activeSlot = 0;

    private void OnEnable()
    {
        activeSlot = 0;

        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        DestroyUIElements(questContent);
        CreateUIElement(questContent, uiElement, QuestList.Instance.questList);

        yield return null; // Ожидание одного кадра для завершения создания элементов

        UpdateUIElementNames();
        QuestInfoUI();
    }

    private void UpdateUIElementNames()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;
        
        for (int i = 0; i < questList.Count; i++)
        {
            var textComponent = questContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();

            textComponent.text = questList[i].questInfo.questName;
        }
    }

    private void QuestInfoUI()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        if (questList.Count == 0)
        {
            questNameUI.text = " ";
            questDescriptionUI.text = " ";
        }
        else
        {
            int goalNumber = questList[activeSlot].questGoalNumber;

            questNameUI.text = $"{questList[activeSlot].questInfo.questName}";

            if (questList[activeSlot].questInfo.questGoal[goalNumber].goalType != QuestGoal.GoalType.End)
            {
                questNameUI.text += $" - {questList[activeSlot].count}/{questList[activeSlot].questInfo.questGoal[goalNumber].neededCount}";
            }

            questDescriptionUI.text = questList[activeSlot].questInfo.questGoal[goalNumber].goal;
        }
    }

    public override void GetItemInfo()
    {
        activeSlot = GetClickedButton() - 1;

        QuestInfoUI();
    }

    public void PassQuest()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        if (questList.Count != 0)
        {
            foreach (QuestSlot quest in questList)
            {
                if (quest.questInfo.questName == questList[activeSlot].questInfo.questName)
                {
                    int goalNumber = quest.questGoalNumber;

                    if (quest.questInfo.questGoal[goalNumber].goalType == QuestGoal.GoalType.End)
                    {
                        FindAnyObjectByType<RankSystem>().LevelUp(quest.questInfo.expReward);

                        Inventory.Instance.AddItem(quest.questInfo.goldRevard);

                        quest.questStage = QuestSlot.QuestStage.Done;
                        QuestList.Instance.RemoveQuest(quest);

                        activeSlot = 0;

                        SoundSystem.Instance.PlaySound("EpicSound");

                        StartCoroutine(UpdateUI());
                        break;
                    }
                }
            }
        }
    }

    public void TrackQuest()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        var questTracking = questUI.GetComponent<QuestTracking>();

        if (!questList[activeSlot].isTracking)
        {
            foreach (QuestSlot quest in questList)
            {
                quest.isTracking = false;
            }

            questList[activeSlot].isTracking = true;
        }
        else
        {
            questList[activeSlot].isTracking = false;
        }
    }
}
