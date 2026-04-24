using System.Collections.Generic;
using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Baza Questów")]
    public List<QuestSO> allQuests = new List<QuestSO>();

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip completeSound;

    public event Action<QuestSO> OnQuestUnlocked;
    public event Action<QuestSO> OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        
        foreach (var q in allQuests) q.ResetState();
    }

    public void UnlockQuest(QuestSO quest)
    {
        if (quest != null && !quest.isUnlocked)
        {
            quest.isUnlocked = true;
            audioSource.PlayOneShot(unlockSound);
            OnQuestUnlocked?.Invoke(quest);
        }
    }

    public void CompleteQuest(QuestSO quest)
    {
        if (quest != null && quest.isUnlocked && !quest.isCompleted)
        {
            quest.isCompleted = true;
            audioSource.PlayOneShot(completeSound);
            OnQuestCompleted?.Invoke(quest);
            CheckAutoUnlocks(quest);
        }
    }

    private void CheckAutoUnlocks(QuestSO completedQuest)
    {
        foreach (QuestSO q in allQuests)
        {
            if (q.previousQuest == completedQuest && !q.isUnlocked)
            {
                UnlockQuest(q);
            }
        }
    }
}