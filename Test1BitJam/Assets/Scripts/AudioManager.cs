using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    
    [Header("Audio Clips")]
    [SerializeField] AudioClip[] music;
    [SerializeField] float fadeDuration;
    [SerializeField] float fadeVolume;
    
    
    [SerializeField] private PlayerStateManager playerStateManager;
    
    private void Start()
    {
        playerStateManager.OnStateChanged += ChangeMusic;
        musicSource.clip = music[0];
        musicSource.Play();
    }

    private void ChangeMusic(PlayerColorState state)
    {
        StartCoroutine(SetMusic(state));
    }

    private IEnumerator SetMusic(PlayerColorState state)
    {
        float startVolume = musicSource.volume;
        float time = musicSource.time;
        Debug.Log(startVolume);

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, fadeVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = music[(int)state];
        musicSource.time = time;
        musicSource.Play();
        
        t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(fadeVolume, startVolume, t / fadeDuration);
            yield return null;
        }
        
        musicSource.volume = startVolume;
    }
}
