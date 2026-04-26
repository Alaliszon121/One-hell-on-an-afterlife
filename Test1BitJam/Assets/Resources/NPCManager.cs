using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NPCManager : MonoBehaviour
{
    private List<DialogueClassifier> classifiers = new List<DialogueClassifier>();
    private int currentQuestionIndex = 0;
    [SerializeField] private string testQuery = "";
    [SerializeField] private TextMeshProUGUI textMeshPro;
    void Start()
    {
        LoadAllModels();
        SetCurrentQuestion(0);
    }

    private void LoadAllModels()
    {
        int index = 0;
        while (true)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>($"Models/model_{index}");

            if (jsonFile == null)
            {
                break;
            }

            classifiers.Add(new DialogueClassifier(jsonFile));
            index++;
        }
    }

    public void SetCurrentQuestion(int questionIndex)
    {
        if (questionIndex >= 0 && questionIndex < classifiers.Count)
        {
            currentQuestionIndex = questionIndex;
        }
        else
        {
            Debug.LogError($"Question index {questionIndex} is out of bounds. Total loaded models: {classifiers.Count}");
        }
    }

    public void ProcessPlayerInput(string playerText)
    {
        if (classifiers.Count == 0)
        {
            Debug.LogError("No models were loaded.");
            return;
        }

        string predictedLabel = classifiers[currentQuestionIndex].Predict(playerText);
        TriggerNPCReaction(predictedLabel);
    }

    private void TriggerNPCReaction(string label)
    {
        Debug.Log(label);
        if (label == "Good_Choice")
        {
            textMeshPro.text = "(Good choice) Exactly! If they wanted rights, they should have just been born rich.";
        }
        else if (label == "Bad_Choice")
        {
            textMeshPro.text = "(Bad choice) Ew. Are you in a union?";
        }
        else
        {
            textMeshPro.text = "(Neutral choice) Did you just quote a socialist manifesto at me? I only speak the language of tax evasion.";
        }
    }

    public void SubmitCurrentInput(TMPro.TMP_InputField inputField)
    {
        string textToProcess = inputField.text;
        ProcessPlayerInput(textToProcess);
        inputField.text = "";
    }
}