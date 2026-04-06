using UnityEngine;
using TMPro;

public class PCInteractable : MonoBehaviour, IInteractable
{
    [Header("Computer References")]
    [Tooltip("The Main Computer UI Canvas or the object holding ComputerManager")]
    [SerializeField] private GameObject computerUIPanel;
    [SerializeField] private GameObject mainComputerUIPanel;

    [Header("Quest Settings")]
    [SerializeField] private bool isQuestTrigger = false;
    [SerializeField] private string questID = "hack_pc_quest";
    [SerializeField] private string bathroom_quest = "6";
    [SerializeField] private string pc_quest = "5";

    [Tooltip("Should this interaction UNLOCK the quest or COMPLETE it?")]
    [SerializeField] private bool completeOnInteraction = false;

    private bool isUsingPC = false;

    private void Start()
    {
        if (computerUIPanel != null)
        {
            computerUIPanel.SetActive(false);
        }
    }

    public void OnInteract(GameObject interactor)
    {
        if (QuestManager.Instance != null)
        {
            Quest quest = QuestManager.Instance.allQuests.Find(q => q.id == questID);
            if (quest != null && quest.isCompleted)
            {
                Debug.Log("This computer has already been hacked.");
                return;
            }
        }

        if (!isUsingPC)
        {
            OpenComputer();
            QuestManager.Instance.UnlockQuest(bathroom_quest);
            QuestManager.Instance.UnlockQuest(pc_quest);
        }
    }

    public string GetInteractText()
    {
        if (QuestManager.Instance != null)
        {
            Quest quest = QuestManager.Instance.allQuests.Find(q => q.id == questID);
            if (quest != null && quest.isCompleted) return "Locked (Hacked)";
        }

        return isUsingPC ? "Exit Computer" : "Use Computer";
    }

    private void OpenComputer()
    {
        if (computerUIPanel == null) return;

        isUsingPC = true;
        computerUIPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void CloseComputer()
    {
        isUsingPC = false;
        computerUIPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}