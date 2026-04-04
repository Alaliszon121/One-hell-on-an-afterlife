using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct TinderProfile
{
    public string nameAndAge;
    public Sprite profilePicture;
    [TextArea] public string bio;
}

public class TinderSwiper : MonoBehaviour
{
    [SerializeField] private ComputerManager manager;

    [Header("UI References")]
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bioText;

    [Header("Profile Database")]
    [SerializeField] private List<TinderProfile> profiles = new List<TinderProfile>();

    private int currentProfileIndex = 0;

    private void OnEnable()
    {
        currentProfileIndex = 0;
        DisplayCurrentProfile();
    }

    private void DisplayCurrentProfile()
    {
        if (profiles.Count == 0) return;

        TinderProfile current = profiles[currentProfileIndex];
        profileImage.sprite = current.profilePicture;
        nameText.text = current.nameAndAge;
        bioText.text = current.bio;
    }

    public void DeclineProfile()
    {
        currentProfileIndex = (currentProfileIndex + 1) % profiles.Count;
        DisplayCurrentProfile();
    }
    
    public void AcceptProfile()
    {
        manager.TriggerMatchAndComplete();
    }
}