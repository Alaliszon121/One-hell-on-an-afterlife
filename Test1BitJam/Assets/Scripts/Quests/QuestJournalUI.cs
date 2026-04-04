using System.Collections.Generic;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject questItemPrefab;

    [Header("Columns (Content Containers)")]
    [SerializeField] private Transform lockedColumn;
    [SerializeField] private Transform activeColumn;
    [SerializeField] private Transform completedColumn;

    private Dictionary<string, QuestUIItem> spawnedItems = new Dictionary<string, QuestUIItem>();

    private void Start()
    {
        journalPanel.SetActive(false);

        QuestManager.Instance.OnQuestUnlocked += HandleQuestUnlocked;
        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;

        InitializeJournal();
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUnlocked -= HandleQuestUnlocked;
            QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            journalPanel.SetActive(!journalPanel.activeSelf);
            Time.timeScale = journalPanel.activeSelf ? 0f : 1f;
        }
    }

    private void InitializeJournal()
    {
        foreach (Quest q in QuestManager.Instance.allQuests)
        {
            GameObject newObj = Instantiate(questItemPrefab);
            QuestUIItem uiItem = newObj.GetComponent<QuestUIItem>();

            uiItem.Setup(q);
            spawnedItems.Add(q.id, uiItem);

            AssignToCorrectColumn(q, uiItem);
        }
    }

    private void AssignToCorrectColumn(Quest quest, QuestUIItem uiItem)
    {
        Transform targetParent = lockedColumn;

        if (quest.isCompleted)
        {
            targetParent = completedColumn;
        }
        else if (quest.isUnlocked)
        {
            targetParent = activeColumn;
        }

        uiItem.transform.SetParent(targetParent, false);

        uiItem.transform.SetAsFirstSibling();
    }

    private void HandleQuestUnlocked(Quest quest)
    {
        if (spawnedItems.TryGetValue(quest.id, out QuestUIItem uiItem))
        {
            uiItem.UpdateStatus(quest);
            AssignToCorrectColumn(quest, uiItem);
        }
    }

    private void HandleQuestCompleted(Quest quest)
    {
        if (spawnedItems.TryGetValue(quest.id, out QuestUIItem uiItem))
        {
            uiItem.UpdateStatus(quest);
            AssignToCorrectColumn(quest, uiItem);
        }
    }
}