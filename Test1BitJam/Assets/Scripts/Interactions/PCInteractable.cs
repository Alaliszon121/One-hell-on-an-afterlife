using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PCInteractable : MonoBehaviour, IInteractable
{
    [Header("Computer References")]
    [Tooltip("The Main Computer UI Canvas or the object holding ComputerManager")]
    [SerializeField] private GameObject computerUIPanel;
    [SerializeField] private GameObject mainComputerUIPanel;

    [Header("Quest Settings")]
    [SerializeField] private bool isQuestTrigger = false;

    [SerializeField] private QuestSO hackPCQuest;
    [SerializeField] private QuestSO bathroomQuest;

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
        if (QuestManager.Instance != null && hackPCQuest != null)
        {
            if (hackPCQuest.isCompleted)
            {
                Debug.Log("This computer has already been hacked.");
                Destroy(this);
                return;
            }
        }

        if (!isUsingPC)
        {
            OpenComputer();
            if (bathroomQuest != null) QuestManager.Instance.UnlockQuest(bathroomQuest);
            if (hackPCQuest != null) QuestManager.Instance.UnlockQuest(hackPCQuest);
        }
    }

    public string GetInteractText()
    {
        if (QuestManager.Instance != null && hackPCQuest != null)
        {
            if (hackPCQuest.isCompleted) return "Locked (Hacked)";
        }

        return isUsingPC ? "Exit Computer" : "Use Computer";
    }

    private void OpenComputer()
    {
        if (computerUIPanel == null) return;

        isUsingPC = true;
        UIManager.Instance.OpenPanel(computerUIPanel);
    }

    public void CloseComputer()
    {
        isUsingPC = false;
        UIManager.Instance.ClosePanel(computerUIPanel);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}