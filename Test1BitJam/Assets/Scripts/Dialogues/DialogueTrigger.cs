using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueSO dialogue;

    public void OnInteract(GameObject interactor)
    {
        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.IsDialogueActive) return;

            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    public string GetInteractText()
    {
        return (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            ? "Kontynuuj"
            : "Rozmawiaj";
    }

    public Transform GetTransform() => transform;
}