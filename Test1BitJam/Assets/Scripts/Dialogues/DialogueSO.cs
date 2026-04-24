using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogi/Dialog")]
public class DialogueSO : ScriptableObject
{
    [Header("Ustawienia Rozmowy")]
    public bool canWalkAway = true;
    public float typingSpeed = 0.03f;

    public List<DialogueLine> lines;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerIcon;
    [TextArea(3, 10)]
    public string text;

    [Header("Audio")]
    [Tooltip("Pojedynczy d³ugi plik z dubbingiem.")]
    public AudioClip dubbingClip;

    [Tooltip("Audio Random Container dla babblingu.")]
    public AudioClip babbleContainer;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueSO nextDialogue;
    public QuestSO questToUnlock;
    public QuestSO questToComplete;
}