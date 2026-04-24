using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueSO mainDialogue;
    [SerializeField] private AudioSource localAudioSource;
    private IdleDialogueController idleController;

    private void Awake()
    {
        idleController = GetComponent<IdleDialogueController>();
        if (localAudioSource == null) localAudioSource = GetComponent<AudioSource>();
    }

    public void OnInteract(GameObject interactor)
    {
        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.IsDialogueActive) return;
            if (idleController != null) idleController.HideIdleDialogue();

            DialogueManager.Instance.StartDialogue(mainDialogue, localAudioSource);

            StartCoroutine(WaitForDialogueEnd());
        }
    }

    private System.Collections.IEnumerator WaitForDialogueEnd()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueActive);
        if (idleController != null) idleController.ShowIdleDialogue();
    }

    public string GetInteractText() => "Rozmawiaj";
    public Transform GetTransform() => transform;
}