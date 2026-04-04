using UnityEngine;

[System.Serializable]
public class Quest
{
    public string id;
    public string title;
    [TextArea(3, 5)]
    public string shortDescription;

    public bool isUnlocked;
    public bool isCompleted;

    [Tooltip("Leave empty if this quest is unlocked via an in-game interaction.")]
    public string previousQuestId;
}