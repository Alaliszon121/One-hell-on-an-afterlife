using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource voiceSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] music;
    [SerializeField] public AudioClip boneSnap;
    [SerializeField] float fadeDuration;
    [SerializeField] float fadeVolume;
    
    void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        DimensionManager.OnDimensionChanged += ChangeMusic;
        musicSource.clip = music[0];
        musicSource.Play();
    }

    private void OnDestroy()
    {
        DimensionManager.OnDimensionChanged -= ChangeMusic;
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

    public void PLaySFX(AudioClip clip)
    {
        if (clip == null) return;
        SFXSource.clip = clip;
        SFXSource.Play();
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null || voiceSource == null) return;
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }

    public void PlayBabble(AudioClip clip)
    {
        if (clip == null || SFXSource == null) return;
        SFXSource.PlayOneShot(clip);
    }
}
