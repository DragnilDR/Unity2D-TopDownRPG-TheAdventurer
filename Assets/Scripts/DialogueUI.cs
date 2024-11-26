using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public Button[] answerButtons;
    public float typingSpeed = 0.02f;

    public DialogueRepository dialogueRepository;
    private DialogueRepository.Dialogue currentDialogue;
    private int currentSentenceIndex = 0;

    private void OnEnable()
    {
        StartDialogue();
    }

    private void StartDialogue()
    {
        currentDialogue = dialogueRepository.dialogues[0];
        currentSentenceIndex = 0;
        StartCoroutine(TypeSentence(currentDialogue.Sentences));
        DisplayAnswers();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogText.text = sentence;
                break;
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void DisplayAnswers()
    {
        if (currentDialogue.Answers != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < currentDialogue.Answers.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.Answers[i].Text;
                    int index = i;
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private async void OnAnswerClicked(int index)
    {
        string responseText = currentDialogue.Answers[index].ReposeText;
        StartCoroutine(TypeSentence(responseText));

        await Task.Delay(1000);

        switch (currentDialogue.Answers[index].actionType)
        {
            case DialogueRepository.Answer.ActionType.NextDialogue:
                currentSentenceIndex++;
                if (currentSentenceIndex < currentDialogue.Answers.Length)
                {
                    StartCoroutine(TypeSentence(currentDialogue.Answers[currentSentenceIndex].Text));
                    DisplayAnswers();
                }
                else
                {
                    CloseDialogueWindow();
                }
                break;
            case DialogueRepository.Answer.ActionType.CloseDialogue:
                CloseDialogueWindow();
                PlayerCamera.Instance.movingLock = false;
                break;
            case DialogueRepository.Answer.ActionType.OpenPassQuest:
                OpenPassQuest();
                break;
            case DialogueRepository.Answer.ActionType.OpenShoop:
                OpenShoop();
                break;
        }
    }

    private void CloseDialogueWindow()
    {
        UIManager.Instance.dialogueWindow.SetActive(false);
    }

    private void OpenPassQuest()
    {
        CloseDialogueWindow();

        UIManager.Instance.ShowUI(UIManager.Instance.passQuestUI);
    }

    private void OpenShoop()
    {
        CloseDialogueWindow();

        UIManager.Instance.ShowUI(UIManager.Instance.shopUI);
    }
}
