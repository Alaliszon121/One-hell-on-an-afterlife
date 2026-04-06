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
    [SerializeField] private bool isQuest = false;
    [SerializeField] private string questID = "0";
    [SerializeField] private bool shouldUnlockQuest = false;
    [SerializeField] private string questToUnlock = "0";
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

    private void EndDialogue()
    {
        isTalking = false;
        currentLineIndex = 0;
        dialogueCanvas.SetActive(false);
        if (shouldUnlockQuest)
        {
            QuestManager.Instance.UnlockQuest(questToUnlock);
        }
        if (isQuest) { 
            QuestManager.Instance.CompleteQuest(questID); 
            isQuest = false; 
        }
        
    }

    public Transform GetTransform()
    {
        return transform;
    }
}