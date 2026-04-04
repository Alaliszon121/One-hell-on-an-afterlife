using System.Collections.Generic;
using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest Database")]
    [Tooltip("Add all your game quests here.")]
    public List<Quest> allQuests = new List<Quest>();

    public event Action<Quest> OnQuestUnlocked;
    public event Action<Quest> OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void UnlockQuest(string questId)
    {
        Quest quest = allQuests.Find(q => q.id == questId);

        if (quest != null && !quest.isUnlocked)
        {
            quest.isUnlocked = true;
            OnQuestUnlocked?.Invoke(quest);
        }
    }

    public void CompleteQuest(string questId)
    {
        Quest quest = allQuests.Find(q => q.id == questId);

        if (quest != null && quest.isUnlocked && !quest.isCompleted)
        {
            quest.isCompleted = true;
            OnQuestCompleted?.Invoke(quest);

            UnlockNextQuests(questId);
        }
    }

    private void UnlockNextQuests(string completedQuestId)
    {
        foreach (Quest q in allQuests)
        {
            if (q.previousQuestId == completedQuestId && !q.isUnlocked)
            {
                UnlockQuest(q.id);
            }
        }
    }
}