using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfo", menuName = "CreateQuestInfo/Quest")]
[System.Serializable]
public class QuestInfoSO : ScriptableObject
{
    [Header("Info")]
    public string questName;
    public List<QuestGoal> questGoal;

    [Header("Rank")]
    public int neededRank;

    [Header("Rewards")]
    public InventorySlot goldRevard;
    public int expReward;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var goal in questGoal)
        {
            goal.UpdateGoal();
        }
    }
#endif
}

[System.Serializable]
public class QuestGoal
{
    public enum GoalType
    {
        Collect,
        Kill,
        End
    }

    [Header("StageType")]
    public GoalType goalType;

    public InventorySlot neededItem;
    public string needdedEnemyName;
    public int neededCount;

    public string goal;

    public void UpdateGoal()
    {
        switch (goalType)
        {
            case GoalType.Collect:
                if (neededItem == null) { return; }

                goal = $"Collect {neededCount} {neededItem.item.itemName}";
                break;
            case GoalType.Kill:
                goal = $"Kill {neededCount} {needdedEnemyName}";
                break;
            case GoalType.End:
                break;
        }
    }
}

