using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;
    public AudioClip[] voiceClips;

    [Header("Settings")]
    public float masterVolume = 1f;
    public float bgmVolume = 0.7f;
    public float sfxVolume = 0.8f;
    public float voiceVolume = 0.9f;

    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> voiceDictionary = new Dictionary<string, AudioClip>();

    private GameConfig gameConfig;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(GameConfig config)
    {
        gameConfig = config;
        LoadAudioClips();
        SetupEventListeners();
        ApplyConfigSettings();
    }

    void SetupAudioSources()
    {
        if (bgmSource == null)
        {
            GameObject bgmObj = new GameObject("BGM Source");
            bgmObj.transform.SetParent(transform);
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (voiceSource == null)
        {
            GameObject voiceObj = new GameObject("Voice Source");
            voiceObj.transform.SetParent(transform);
            voiceSource = voiceObj.AddComponent<AudioSource>();
            voiceSource.loop = false;
            voiceSource.playOnAwake = false;
        }
    }

    void LoadAudioClips()
    {
        // BGMクリップをディクショナリに登録
        foreach (var clip in bgmClips)
        {
            if (clip != null)
                bgmDictionary[clip.name] = clip;
        }

        // SFXクリップをディクショナリに登録
        foreach (var clip in sfxClips)
        {
            if (clip != null)
                sfxDictionary[clip.name] = clip;
        }

        // Voiceクリップをディクショナリに登録
        foreach (var clip in voiceClips)
        {
            if (clip != null)
                voiceDictionary[clip.name] = clip;
        }
    }

    void SetupEventListeners()
    {
        GameEvents.OnSFXPlay += PlaySFX;
        GameEvents.OnBGMPlay += PlayBGM;
        GameEvents.OnVoicePlay += (name, position) => PlayVoice(name);
    }

    void ApplyConfigSettings()
    {
        if (gameConfig != null)
        {
            bgmVolume = gameConfig.bgmVolume;
            sfxVolume = gameConfig.sfxVolume;
            voiceVolume = gameConfig.voiceVolume;

            UpdateVolumes();
        }
    }

    public void PlayBGM(string clipName)
    {
        if (bgmDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            if (bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"BGM clip '{clipName}' not found!");
        }
    }

    public void PlaySFX(string clipName)
    {
        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip '{clipName}' not found!");
        }
    }

    public void PlayVoice(string clipName)
    {
        if (voiceDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            voiceSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Voice clip '{clipName}' not found!");
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopAllSFX()
    {
        sfxSource.Stop();
    }

    public void StopVoice()
    {
        voiceSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    void UpdateVolumes()
    {
        bgmSource.volume = bgmVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
        voiceSource.volume = voiceVolume * masterVolume;
    }

    void OnDestroy()
    {
        GameEvents.OnSFXPlay -= PlaySFX;
        GameEvents.OnBGMPlay -= PlayBGM;
        GameEvents.OnVoicePlay -= (name, position) => PlayVoice(name);
    }
}