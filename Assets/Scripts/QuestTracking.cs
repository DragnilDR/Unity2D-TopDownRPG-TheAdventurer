using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestTracking : MonoBehaviour
{
    private TextMeshProUGUI questUI;

    private void Start()
    {
        questUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        List<QuestSlot> questList = QuestList.Instance.questList;

        foreach (QuestSlot slot in questList)
        {
            if (slot.isTracking)
            {
                int goalNumber = slot.questGoalNumber;
                questUI.text = $"{slot.questInfo.questGoal[goalNumber].goal}";

                if (slot.questInfo.questGoal[goalNumber].goalType != QuestGoal.GoalType.End)
                {
                    questUI.text += $"\n{slot.count}/{slot.questInfo.questGoal[goalNumber].neededCount}";
                }
                break;
            }
            else questUI.text = string.Empty;
        }
    }
}
