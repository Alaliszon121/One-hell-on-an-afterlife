using System.Collections.Generic;
using System;
using UnityEngine;

public partial class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest Database")]
    public List<Quest> allQuests = new List<Quest>();

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip completeSound;

    public event Action<Quest> OnQuestUnlocked;
    public event Action<Quest> OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void UnlockQuest(string questId)
    {
        Quest quest = allQuests.Find(q => q.id == questId);
        if (quest != null && !quest.isUnlocked)
        {
            quest.isUnlocked = true;
            audioSource.PlayOneShot(unlockSound);
            OnQuestUnlocked?.Invoke(quest);
        }
    }

    public void CompleteQuest(string questId)
    {
        Quest quest = allQuests.Find(q => q.id == questId);
        if (quest != null && quest.isUnlocked && !quest.isCompleted)
        {
            quest.isCompleted = true;
            audioSource.PlayOneShot(completeSound);
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