using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRepository : MonoBehaviour
{
    [System.Serializable]
    public struct Dialogue
    {
        [field: SerializeField] public string Tag;
        [field: SerializeField] public string Sentences;
        [field: SerializeField] public Answer[] Answers;
    }

    [System.Serializable]
    public struct Answer
    {
        [field: SerializeField] public string Text;
        [field: SerializeField] public string ReposeText;
        [field: SerializeField] public ActionType actionType;

        public enum ActionType
        {
            NextDialogue,
            CloseDialogue,
            OpenPassQuest,
            OpenShoop
        }
    }

    public Dialogue[] dialogues;
}
