using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Button openJournalButton;

    [Header("Input Settings")]
    [Tooltip("Akcja do otwierania/zamykania dziennika (np. przypisana do klawisza J lub przycisku na padzie)")]
    [SerializeField] private InputActionReference toggleJournalAction;

    [Header("Quest Trigger")]
    [Tooltip("The Quest to complete when opening journal for the first time")]
    [SerializeField] private QuestSO openJournalQuest;

    [Header("Columns")]
    [SerializeField] private Transform lockedColumn;
    [SerializeField] private Transform activeColumn;
    [SerializeField] private Transform completedColumn;

    private Dictionary<QuestSO, QuestUIItem> spawnedItems = new Dictionary<QuestSO, QuestUIItem>();
    private Image buttonImage;
    private bool needsAttention = false;
    private bool isCurrentlyShaking = false;
    private Vector3 buttonOriginalPos;
    private bool wasOpenedBefore = false;

    private void OnEnable()
    {
        if (toggleJournalAction != null)
        {
            toggleJournalAction.action.Enable();
            toggleJournalAction.action.performed += HandleJournalInput;
        }
    }

    private void OnDisable()
    {
        if (toggleJournalAction != null)
        {
            toggleJournalAction.action.performed -= HandleJournalInput;
            toggleJournalAction.action.Disable();
        }
    }

    private void HandleJournalInput(InputAction.CallbackContext context)
    {
        ToggleJournal();
    }

    private void Start()
    {
        gameObject.SetActive(true);
        buttonOriginalPos = openJournalButton.transform.localPosition;
        buttonImage = openJournalButton.GetComponent<Image>();

        openJournalButton.onClick.AddListener(ToggleJournal);

        QuestManager.Instance.OnQuestUnlocked += HandleUpdate;
        QuestManager.Instance.OnQuestCompleted += HandleUpdate;

        DimensionManager.OnDimensionChanged += HandleStateChanged;

        InitializeJournal();
        UpdateJournalButtonColor();

        if (openJournalQuest != null)
        {
            QuestManager.Instance.UnlockQuest(openJournalQuest);
        }

        ToggleJournal();
    }

    private void OnDestroy()
    {
        DimensionManager.OnDimensionChanged -= HandleStateChanged;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUnlocked -= HandleUpdate;
            QuestManager.Instance.OnQuestCompleted -= HandleUpdate;
        }
    }

    private void Update()
    {
        if (needsAttention && Time.timeScale > 0 && !isCurrentlyShaking)
        {
            StartCoroutine(ShakeJournalButton());
        }
    }

    public void ToggleJournal()
    {
        if (!wasOpenedBefore)
        {
            if (openJournalQuest != null)
            {
                QuestManager.Instance.CompleteQuest(openJournalQuest);
            }
            wasOpenedBefore = true;
        }

        UIManager.Instance.TogglePanel(journalPanel);

        bool isOpen = journalPanel.activeSelf;

        UpdateJournalButtonColor();

        if (isOpen)
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
        if (buttonImage == null) return;

        PlayerColorState currentState = DimensionManager.CurrentState;

        if (journalPanel.activeSelf || currentState == PlayerColorState.White)
            buttonImage.color = Color.white;
        else if (currentState == PlayerColorState.Blue)
            buttonImage.color = new Color(0.5f, 0.7f, 1f);
        else if (currentState == PlayerColorState.Red)
            buttonImage.color = new Color(1f, 0.5f, 0.5f);
    }

    private void HandleUpdate(QuestSO quest)
    {
        if (spawnedItems.TryGetValue(quest, out QuestUIItem uiItem))
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
        foreach (QuestSO q in QuestManager.Instance.allQuests)
        {
            GameObject newObj = Instantiate(questItemPrefab);
            QuestUIItem uiItem = newObj.GetComponent<QuestUIItem>();
            uiItem.Setup(q);

            spawnedItems.Add(q, uiItem);
            AssignToCorrectColumn(q, uiItem);
        }
    }

    private void AssignToCorrectColumn(QuestSO quest, QuestUIItem uiItem)
    {
        Transform targetParent = quest.isCompleted ? completedColumn : (quest.isUnlocked ? activeColumn : lockedColumn);
        uiItem.transform.SetParent(targetParent, false);
        uiItem.transform.SetAsFirstSibling();
    }
}