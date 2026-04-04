using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComputerLogin : MonoBehaviour
{
    [SerializeField] private ComputerManager manager;
    [SerializeField] private TMP_InputField pinInputField;
    [SerializeField] private string correctPin = "1234";

    private RectTransform inputFieldRect;
    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        inputFieldRect = pinInputField.GetComponent<RectTransform>();
        originalPosition = inputFieldRect.localPosition;

        pinInputField.characterLimit = 4;
        pinInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
    }

    public void OnSubmitClicked()
    {
        if (isShaking) return;

        if (pinInputField.text == correctPin)
        {
            pinInputField.text = "";
            manager.ShowPanel(manager.desktopPanel);
        }
        else
        {
            StartCoroutine(ShakeInputField());
        }
    }

    private IEnumerator ShakeInputField()
    {
        isShaking = true;
        float duration = 0.4f;
        float magnitude = 15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
            inputFieldRect.localPosition = new Vector3(x, originalPosition.y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        inputFieldRect.localPosition = originalPosition;
        pinInputField.text = "";
        isShaking = false;
    }
}