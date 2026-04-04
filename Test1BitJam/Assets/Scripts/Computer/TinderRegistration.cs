using UnityEngine;
using TMPro;

public class TinderRegistration : MonoBehaviour
{
    [SerializeField] private ComputerManager manager;
    [SerializeField] private TMP_InputField bossNameInput;
    [SerializeField] private TextMeshProUGUI generatedEmailText;

    private void Awake()
    {
        bossNameInput.onValueChanged.AddListener(GenerateEmail);
    }

    private void GenerateEmail(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            generatedEmailText.text = "";
            return;
        }

        string formattedName = input.ToLower().Replace(" ", ".");
        generatedEmailText.text = $"{formattedName}@bossmail.com";
    }

    public void OnCreateAccountClicked()
    {
        if (!string.IsNullOrEmpty(bossNameInput.text))
        {
            manager.ShowPanel(manager.tinderSwipePanel);
        }
    }
}