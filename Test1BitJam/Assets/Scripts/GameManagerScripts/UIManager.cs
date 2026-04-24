using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private HashSet<GameObject> pausingPanels = new HashSet<GameObject>();
    private HashSet<GameObject> nonPausingPanels = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 1. Wymuszamy, aby kursor by³ ZAWSZE widoczny i odblokowany od samego startu gry
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenPanel(GameObject panel, bool pausesGame = true)
    {
        if (panel == null) return;

        panel.SetActive(true);

        if (pausesGame)
        {
            pausingPanels.Add(panel);
        }
        else
        {
            nonPausingPanels.Add(panel);
        }

        UpdateGameState();
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(false);

        pausingPanels.Remove(panel);
        nonPausingPanels.Remove(panel);

        UpdateGameState();
    }

    public void TogglePanel(GameObject panel, bool pausesGame = true)
    {
        if (panel.activeSelf)
        {
            ClosePanel(panel);
        }
        else
        {
            OpenPanel(panel, pausesGame);
        }
    }

    private void UpdateGameState()
    {
        if (pausingPanels.Count > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}