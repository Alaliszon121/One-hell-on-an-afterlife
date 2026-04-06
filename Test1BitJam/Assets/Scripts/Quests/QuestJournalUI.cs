using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Button openJournalButton;

    [Header("Columns")]
    [SerializeField] private Transform lockedColumn;
    [SerializeField] private Transform activeColumn;
    [SerializeField] private Transform completedColumn;

    private Dictionary<string, QuestUIItem> spawnedItems = new Dictionary<string, QuestUIItem>();

    private bool needsAttention = false;
    private bool isCurrentlyShaking = false;
    private Vector3 buttonOriginalPos;

    private void Start()
    {
        journalPanel.SetActive(false);
        buttonOriginalPos = openJournalButton.transform.localPosition;

        openJournalButton.onClick.AddListener(ToggleJournal);

        QuestManager.Instance.OnQuestUnlocked += (q) => { HandleUpdate(q); };
        QuestManager.Instance.OnQuestCompleted += (q) => { HandleUpdate(q); };

        InitializeJournal();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }

        if (needsAttention && Time.timeScale > 0 && !isCurrentlyShaking)
        {
            StartCoroutine(ShakeJournalButton());
        }
    }

    public void ToggleJournal()
    {
        bool newState = !journalPanel.activeSelf;
        journalPanel.SetActive(newState);
        Time.timeScale = newState ? 0f : 1f;

        if (newState)
        {
            needsAttention = false;
            StopAllCoroutines();
            isCurrentlyShaking = false;
            openJournalButton.transform.localPosition = buttonOriginalPos;
        }
    }

    private void HandleUpdate(Quest quest)
    {
        if (spawnedItems.TryGetValue(quest.id, out QuestUIItem uiItem))
        {
            uiItem.UpdateStatus(quest);
            AssignToCorrectColumn(quest, uiItem);

            if (!journalPanel.activeSelf)
            {
                needsAttention = true;
            }
        }
    }

    private IEnumerator ShakeJournalButton()
    {
        isCurrentlyShaking = true;
        float duration = 0.5f;
        float magnitude = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = buttonOriginalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = buttonOriginalPos.y + Random.Range(-1f, 1f) * magnitude;

            openJournalButton.transform.localPosition = new Vector3(x, y, buttonOriginalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        openJournalButton.transform.localPosition = buttonOriginalPos;

        yield return new WaitForSeconds(2f);
        isCurrentlyShaking = false;
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
        Transform targetParent = quest.isCompleted ? completedColumn : (quest.isUnlocked ? activeColumn : lockedColumn);
        uiItem.transform.SetParent(targetParent, false);
        uiItem.transform.SetAsFirstSibling();
    }
}