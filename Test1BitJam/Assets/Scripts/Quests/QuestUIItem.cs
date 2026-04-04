using UnityEngine;
using TMPro;

public class QuestUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statusText;

    public void Setup(Quest quest)
    {
        UpdateStatus(quest);
    }

    public void UpdateStatus(Quest quest)
    {
        if (quest.isCompleted)
        {
            statusText.text = "[Completed]";
            statusText.color = Color.green;
            titleText.text = quest.title;
            descriptionText.text = quest.shortDescription;
        }
        else if (quest.isUnlocked)
        {
            statusText.text = "[Active]";
            statusText.color = Color.white;
            titleText.text = quest.title;
            descriptionText.text = quest.shortDescription;
        }
        else
        {
            statusText.text = "[Locked]";
            statusText.color = Color.gray;

            if (quest.hideInfoWhenLocked)
            {
                titleText.text = "???";
                descriptionText.text = "Keep playing to reveal this objective.";
            }
            else
            {
                titleText.text = quest.title;
                descriptionText.text = quest.shortDescription;
            }
        }
    }
}