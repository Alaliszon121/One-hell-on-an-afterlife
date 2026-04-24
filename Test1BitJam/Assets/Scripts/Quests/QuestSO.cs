using UnityEngine;

[CreateAssetMenu(fileName = "NowyQuest", menuName = "System Questów/Quest")]
public class QuestSO : ScriptableObject
{
    [Header("Informacje o zadaniu")]
    public string title;
    [TextArea(3, 5)]
    public string shortDescription;

    [Header("Logika")]
    [Tooltip("Zadanie, które musi zostaæ ukoñczone, aby to sta³o siê aktywne.")]
    public QuestSO previousQuest;
    public bool hideInfoWhenLocked = true;

    [System.NonSerialized] public bool isUnlocked;
    [System.NonSerialized] public bool isCompleted;

    public void ResetState()
    {
        isUnlocked = false;
        isCompleted = false;
    }
}