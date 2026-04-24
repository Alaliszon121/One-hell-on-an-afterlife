using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI activeQuestText;

    private List<QuestSO> activeQuests = new List<QuestSO>();

    private void Start()
    {
        if (QuestManager.Instance != null && QuestManager.Instance.allQuests != null)
        {
            foreach (QuestSO quest in QuestManager.Instance.allQuests)
            {
                if (quest.isUnlocked && !quest.isCompleted)
                {
                    activeQuests.Add(quest);
                }
            }
        }

        UpdateTrackerDisplay();

        QuestManager.Instance.OnQuestUnlocked += AddActiveQuest;
        QuestManager.Instance.OnQuestCompleted += RemoveCompletedQuest;
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUnlocked -= AddActiveQuest;
            QuestManager.Instance.OnQuestCompleted -= RemoveCompletedQuest;
        }
    }

    private void AddActiveQuest(QuestSO quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            UpdateTrackerDisplay();
        }
    }

    private void RemoveCompletedQuest(QuestSO quest)
    {
        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            UpdateTrackerDisplay();
        }
    }

    private void UpdateTrackerDisplay()
    {
        if (activeQuests.Count > 0)
        {
            QuestSO latestQuest = activeQuests[activeQuests.Count - 1];
            activeQuestText.text = $"{latestQuest.shortDescription}";
        }
        else
        {
            activeQuestText.text = "No task in progress";
        }
    }
}