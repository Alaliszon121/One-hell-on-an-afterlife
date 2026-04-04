using System.Collections.Generic;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject questItemPrefab;

    private Dictionary<string, QuestUIItem> spawnedItems = new Dictionary<string, QuestUIItem>();

    private void Start()
    {
        journalPanel.SetActive(false);

        QuestManager.Instance.OnQuestUnlocked += HandleQuestUnlocked;
        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
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

    private void HandleQuestUnlocked(Quest quest)
    {
        GameObject newObj = Instantiate(questItemPrefab, contentContainer);
        QuestUIItem uiItem = newObj.GetComponent<QuestUIItem>();

        uiItem.Setup(quest);

        newObj.transform.SetAsFirstSibling();

        spawnedItems.Add(quest.id, uiItem);
    }

    private void HandleQuestCompleted(Quest quest)
    {
        if (spawnedItems.TryGetValue(quest.id, out QuestUIItem uiItem))
        {
            uiItem.UpdateStatus(quest);
        }
    }
}