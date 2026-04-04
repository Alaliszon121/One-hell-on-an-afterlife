using UnityEngine;
using TMPro;

public class QuestUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statusText;
    public void Setup(Quest quest)
    {
        titleText.text = quest.title;
        descriptionText.text = quest.shortDescription;
        UpdateStatus(quest);
    }
    public void UpdateStatus(Quest quest)
    {
        statusText.text = quest.isCompleted ? "[Completed]" : "[Active]";
        statusText.color = quest.isCompleted ? Color.gray : Color.white;
    }
}