using UnityEngine;

public class ComputerManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject desktopPanel;
    public GameObject tinderRegistrationPanel;
    public GameObject tinderSwipePanel;
    public GameObject dummyAppPanel;
    public GameObject matchPopupPanel;

    [Header("Quest Integration")]
    [Tooltip("The ID of the quest to complete upon matching")]
    public string hackingQuestId = "hack_pc_quest";

    private bool isPcLocked = false;

    private void OnEnable()
    {
        if (isPcLocked)
        {
            CloseComputer();
            return;
        }

        ShowPanel(loginPanel);
        Time.timeScale = 0f;
    }

    public void ShowPanel(GameObject panelToShow)
    {
        loginPanel.SetActive(panelToShow == loginPanel);
        desktopPanel.SetActive(panelToShow == desktopPanel);
        tinderRegistrationPanel.SetActive(panelToShow == tinderRegistrationPanel);
        tinderSwipePanel.SetActive(panelToShow == tinderSwipePanel);
        dummyAppPanel.SetActive(panelToShow == dummyAppPanel);
        matchPopupPanel.SetActive(panelToShow == matchPopupPanel);
    }

    public void TriggerMatchAndComplete()
    {
        ShowPanel(matchPopupPanel);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CompleteQuest(hackingQuestId);
        }

        isPcLocked = true;
        Time.timeScale = 1f;
        Invoke(nameof(CloseComputer), 3f);
    }

    public void CloseComputer()
    {
        gameObject.SetActive(false);
    }
}