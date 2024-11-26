using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public List<QuestSlot> questSlot = new();// поменять на Scriptable Obj

    [SerializeField] private GameObject questInfoUI;
    [SerializeField] private GameObject helpWindow;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;

    private int activeSlot = 0;

    private void Start()
    {
        GenerateQuest();
    }

    private void Update()
    {
        UpdateGoal();

        if (questInfoUI.activeSelf == true)
        {
            GetQuestInfo();
        }

        if (gameObject.activeSelf == true)
        {
            GetCountItemForQuest();
            GenerateNewQuest();
        }
    }

    private void UpdateGoal()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        foreach (var goal in questList)
        {
            int goalNumber = goal.questGoalNumber;

            if (goal.questInfo.questGoal[goalNumber].goalType != QuestGoal.GoalType.End)
                if (goal.count >= goal.questInfo.questGoal[goalNumber].neededCount)
                {
                    if (goal.questInfo.questGoal[goalNumber].goalType == QuestGoal.GoalType.Collect)
                    {
                        Inventory.Instance.RemoveItem(goal.questInfo.questGoal[goalNumber].neededItem, goal.questInfo.questGoal[goalNumber].neededCount);
                    }
                    goal.count -= goal.questInfo.questGoal[goalNumber].neededCount;

                    goal.questGoalNumber++;
                    break;
                }
        }
    }

    private void GenerateQuest()
    {
        questSlot.Clear();

        List<QuestInfoSO> allQuests = new(Resources.LoadAll<QuestInfoSO>("QuestInfo"));

        foreach (QuestInfoSO quest in allQuests)
        {
            if (quest.neededRank <= RankSystem.Instance.currentLevel + 1)
            {
                QuestSlot slot = new()
                {
                    questInfo =  quest
                };

                questSlot.Add(slot);
            }
        }
    }

    private void GenerateNewQuest()
    {
        if (QuestBoardUI.Instance)
        {
            activeSlot = QuestBoardUI.Instance.activeSlot;
        }

        if (questSlot[activeSlot].questStage == QuestSlot.QuestStage.Done)
        {
            questSlot[activeSlot].count = 0;
            QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("QuestInfo");
            List<QuestInfoSO> quests = new();
            foreach (var quest in allQuests)
            {
                if (quest != questSlot[activeSlot].questInfo)
                {
                    quests.Add(quest);
                }
            }
            questSlot[activeSlot].questInfo = quests[Random.Range(0, quests.Count)];

            questSlot[activeSlot].questStage = QuestSlot.QuestStage.Waiting;
        }
    }

    public void GetCountItemForQuest()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        foreach (var quest in questList)
        {
            int goalNumber = quest.questGoalNumber;

            if (quest.questInfo.questGoal[goalNumber].goalType == QuestGoal.GoalType.Collect)
            {
                if (Inventory.Instance.items.Count != 0)
                {
                    InventorySlot item = Inventory.Instance.items.Find(item => item.item == quest.questInfo.questGoal[goalNumber].neededItem.item);

                    if (item != null)
                        quest.count = item.count;
                    else quest.count = 0;
                }
                else quest.count = 0;
            }
        }
    }

    public void GetQuestInfo()
    {
        questName.text = questSlot[activeSlot].questInfo.questName;
        questDescription.text = questSlot[activeSlot].questInfo.questGoal[0].goal;
    }

    public void TakeQuest()
    {
        SoundSystem.Instance.PlaySound("TurnPage");

        if (questSlot[activeSlot].questInfo.neededRank <= RankSystem.Instance.currentLevel)
        {
            if (questSlot[activeSlot].questStage == QuestSlot.QuestStage.Waiting)
            {
                QuestList.Instance.AddQuest(questSlot[activeSlot]);
                questSlot[activeSlot].questStage = QuestSlot.QuestStage.Progressing;
            }
        }
        else
        {
            helpWindow.SetActive(true);
            PlayerCamera.Instance.movingLock = true;
        }
    }

    public void CloseHelpWindow()
    {
        PlayerCamera.Instance.movingLock = false;
        helpWindow.SetActive(false);
    }

    public void PutQuest()
    {
        SoundSystem.Instance.PlaySound("TurnPage");

        QuestList.Instance.RemoveQuest(QuestList.Instance.questList.Find(quest => quest.questInfo == questSlot[activeSlot].questInfo));
        questSlot[activeSlot].questStage = QuestSlot.QuestStage.Waiting;
    }
}
