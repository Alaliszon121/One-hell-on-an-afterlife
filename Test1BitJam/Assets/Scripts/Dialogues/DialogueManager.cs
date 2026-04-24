using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Image speakerIcon;
    public Transform choiceContainer;
    public GameObject choicePrefab;

    [Header("Input Settings")]
    public InputActionReference continueAction;

    private Coroutine currentDialogueCoroutine;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullText;

    public DialogueSO currentDialogue { get; private set; }
    public bool IsDialogueActive => dialoguePanel.activeSelf;
    public bool BlocksMovement => currentDialogue != null && !currentDialogue.canWalkAway;

    private void Awake() => Instance = this;

    private void OnEnable() => continueAction?.action.Enable();
    private void OnDisable() => continueAction?.action.Disable();

    public void StartDialogue(DialogueSO dialogue)
    {
        if (IsDialogueActive && currentDialogue == dialogue) return;

        currentDialogue = dialogue;
        if (currentDialogueCoroutine != null) StopCoroutine(currentDialogueCoroutine);

        ClearChoices();
        UIManager.Instance.OpenPanel(dialoguePanel, false);
        currentDialogueCoroutine = StartCoroutine(PlayDialogue(dialogue));
    }

    IEnumerator PlayDialogue(DialogueSO dialogue)
    {
        foreach (var line in dialogue.lines)
        {
            nameText.text = line.speakerName;

            if (line.speakerIcon != null)
            {
                speakerIcon.gameObject.SetActive(true);
                speakerIcon.sprite = line.speakerIcon;
            }
            else
            {
                speakerIcon.gameObject.SetActive(false);
            }

            if (line.voiceClip != null) AudioManager.instance.PLaySFX(line.voiceClip);

            currentFullText = line.text;
            typingCoroutine = StartCoroutine(TypeEffect(currentFullText, dialogue.typingSpeed));

            yield return new WaitUntil(() => !isTyping || continueAction.action.WasPressedThisFrame());

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                isTyping = false;
                bodyText.text = currentFullText;

                yield return null;
            }

            yield return new WaitUntil(() => continueAction.action.WasPressedThisFrame());
        }

        if (dialogue.choices.Count > 0)
        {
            ShowChoices(dialogue.choices);
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeEffect(string text, float speed)
    {
        isTyping = true;
        bodyText.text = "";
        foreach (char c in text.ToCharArray())
        {
            bodyText.text += c;
            yield return new WaitForSecondsRealtime(speed);
        }
        isTyping = false;
    }

    private void ShowChoices(List<DialogueChoice> choices)
    {
        ClearChoices();
        foreach (DialogueChoice choice in choices)
        {
            GameObject choiceObj = Instantiate(choicePrefab, choiceContainer);
            TextMeshProUGUI choiceText = choiceObj.GetComponentInChildren<TextMeshProUGUI>();
            if (choiceText != null) choiceText.text = choice.choiceText;

            Button button = choiceObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        ClearChoices();
        if (choice.questToUnlock != null && QuestManager.Instance != null)
            QuestManager.Instance.UnlockQuest(choice.questToUnlock);
        if (choice.questToComplete != null && QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest(choice.questToComplete);
        
        if (choice.nextDialogue != null)
            currentDialogueCoroutine = StartCoroutine(PlayDialogue(choice.nextDialogue));
        else
            EndDialogue();
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public void ForceCloseDialogue()
    {
        if (currentDialogueCoroutine != null) StopCoroutine(currentDialogueCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        ClearChoices();
        EndDialogue();
    }

    private void EndDialogue()
    {
        UIManager.Instance.ClosePanel(dialoguePanel);
        currentDialogueCoroutine = null;
        typingCoroutine = null;
        currentDialogue = null;
    }
}