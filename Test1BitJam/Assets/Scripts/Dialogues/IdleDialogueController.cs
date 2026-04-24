using UnityEngine;
using TMPro;
using System.Collections;

public class IdleDialogueController : MonoBehaviour
{
    [Header("Ustawienia Danych")]
    [SerializeField] private DialogueSO idleDialogue;
    [SerializeField] private float delayBetweenLines = 3f;

    [Header("UI References (World Space)")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Audio")]
    [SerializeField] private AudioSource localAudioSource;

    private Coroutine idleCoroutine;
    private bool isSuppressed = false;

    private void Awake()
    {
        if (localAudioSource == null) localAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (idleDialogue != null && dialogueCanvas != null) ShowIdleDialogue();
    }

    public void ShowIdleDialogue()
    {
        isSuppressed = false;
        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        dialogueCanvas.SetActive(true);
        idleCoroutine = StartCoroutine(IdleLoop());
    }

    public void HideIdleDialogue()
    {
        isSuppressed = true;
        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        if (localAudioSource != null) localAudioSource.Stop();
        if (AudioManager.instance != null) AudioManager.instance.StopVoice();
        dialogueCanvas.SetActive(false);
    }

    IEnumerator IdleLoop()
    {
        int currentLine = 0;
        while (!isSuppressed)
        {
            DialogueLine line = idleDialogue.lines[currentLine];

            if (AudioManager.instance != null) AudioManager.instance.PlayVoice(line.dubbingClip);
            if (localAudioSource != null && line.babbleContainer != null)
            {
                localAudioSource.resource = line.babbleContainer;
                localAudioSource.Play();
            }

            yield return StartCoroutine(TypeEffect(line.text, idleDialogue.typingSpeed));

            if (localAudioSource != null) localAudioSource.Stop();
            if (AudioManager.instance != null) AudioManager.instance.StopVoice();

            yield return new WaitForSeconds(delayBetweenLines);
            currentLine = (currentLine + 1) % idleDialogue.lines.Count;
        }
    }

    IEnumerator TypeEffect(string text, float speed)
    {
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }
    }
}