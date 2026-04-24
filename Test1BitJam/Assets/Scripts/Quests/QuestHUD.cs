using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI activeQuestText;

    private void Start()
    {
        QuestManager.Instance.OnQuestUnlocked += UpdateTracker;
        QuestManager.Instance.OnQuestCompleted += (q) => activeQuestText.text = "Brak zadañ";
    }

    private void UpdateTracker(QuestSO quest)
    {
        activeQuestText.text = $"Cel: {quest.title}";
    }
}