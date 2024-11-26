using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSlot
{
    public QuestInfoSO questInfo;
    public int questGoalNumber;
    public int count;
    public enum QuestStage
    {
        Waiting,
        Progressing,
        Done
    }

    public QuestStage questStage;

    public bool isTracking;
}

public class QuestList : MonoBehaviour
{
    public static QuestList Instance { get; private set; }

    public List<QuestSlot> questList = new();

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else Destroy(gameObject);
    }

    public void AddQuest(QuestSlot quest)
    {
        questList.Add(quest);
    }

    public void RemoveQuest(QuestSlot quest)
    {
        questList.Remove(quest);
    }
}
