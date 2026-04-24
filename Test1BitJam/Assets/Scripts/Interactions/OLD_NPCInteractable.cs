using UnityEngine;
using TMPro;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("Dialogue References")]
    [Tooltip("The World Space Canvas GameObject")]
    [SerializeField] private GameObject dialogueCanvas;
    [Tooltip("The TextMeshPro UI element inside the Canvas")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Content")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    private int currentLineIndex = 0;
    private bool isTalking = false;

    private void Start()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }
    }

    public void OnInteract(GameObject interactor)
    {
        if (!isTalking)
        {
            StartDialogue();
        }
        else
        {
            AdvanceDialogue();
        }
    }

    public string GetInteractText()
    {
        return isTalking ? "Continue" : "Talk";
    }

    private void StartDialogue()
    {
        if (dialogueLines.Length == 0) return;

        isTalking = true;
        currentLineIndex = 0;
        dialogueText.text = dialogueLines[currentLineIndex];
        dialogueCanvas.SetActive(true);
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    [SerializeField] private QuestSO questReference;
    [SerializeField] private bool isQuestCompletionTrigger;
    [SerializeField] private bool isQuestUnlockTrigger = false;

    private void EndDialogue()
    {
        isTalking = false;
        dialogueCanvas.SetActive(false);

        if (questReference != null)
        {
            if (isQuestUnlockTrigger)
                QuestManager.Instance.UnlockQuest(questReference);
            if (isQuestCompletionTrigger)
                QuestManager.Instance.CompleteQuest(questReference);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}