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

    [Header("Dependencies")]
    [SerializeField] private PlayerStateManager playerStateManager;

    [Header("Columns")]
    [SerializeField] private Transform lockedColumn;
    [SerializeField] private Transform activeColumn;
    [SerializeField] private Transform completedColumn;

    private Dictionary<string, QuestUIItem> spawnedItems = new Dictionary<string, QuestUIItem>();
    private Image buttonImage;
    private bool needsAttention = false;
    private bool isCurrentlyShaking = false;
    private Vector3 buttonOriginalPos;
    private bool wasOpenedBefore = false;

    private void Start()
    {
        journalPanel.SetActive(true);
        buttonOriginalPos = openJournalButton.transform.localPosition;
        buttonImage = openJournalButton.GetComponent<Image>();

        openJournalButton.onClick.AddListener(ToggleJournal);

        QuestManager.Instance.OnQuestUnlocked += (q) => { HandleUpdate(q); };
        QuestManager.Instance.OnQuestCompleted += (q) => { HandleUpdate(q); };

        if (playerStateManager != null)
        {
            playerStateManager.OnStateChanged += HandleStateChanged;
        }

        InitializeJournal();
        UpdateJournalButtonColor();
    }

    private void OnDestroy()
    {
        if (playerStateManager != null)
        {
            playerStateManager.OnStateChanged -= HandleStateChanged;
        }
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
        if (!wasOpenedBefore)
        {
            QuestManager.Instance.CompleteQuest("0");
            wasOpenedBefore = true;
        }

        bool newState = !journalPanel.activeSelf;
        journalPanel.SetActive(newState);
        Time.timeScale = newState ? 0f : 1f;

        UpdateJournalButtonColor();

        if (newState)
        {
            needsAttention = false;
            StopAllCoroutines();
            isCurrentlyShaking = false;
            openJournalButton.transform.localPosition = buttonOriginalPos;
        }
    }

    private void HandleStateChanged(PlayerColorState newState)
    {
        UpdateJournalButtonColor();
    }

    private void UpdateJournalButtonColor()
    {
        if (buttonImage == null || playerStateManager == null) return;

        if (journalPanel.activeSelf || playerStateManager.currentState == PlayerColorState.White)
        {
            buttonImage.color = Color.white;
        }
        else if (playerStateManager.currentState == PlayerColorState.Blue)
        {
            buttonImage.color = new Color(0.5f, 0.7f, 1f);
        }
        else if (playerStateManager.currentState == PlayerColorState.Red)
        {
            buttonImage.color = new Color(1f, 0.5f, 0.5f);
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