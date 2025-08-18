using UnityEngine;
using System.Collections;

public class CharacterAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private Character character;

    [Header("Audio Settings")]
    public float sfxVolume = 1f;
    public float voiceVolume = 1f;
    public bool enableAudio = true;

    [Header("Audio Clips")]
    public AudioClip[] attackSounds;
    public AudioClip[] damagedSounds;
    public AudioClip[] healSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] skillSounds;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    public void Initialize(Character owner)
    {
        character = owner;

        // キャラクターデータから音声を設定
        if (character.characterData != null)
        {
            if (character.characterData.attackSounds != null)
                attackSounds = character.characterData.attackSounds;
            if (character.characterData.damagedSounds != null)
                damagedSounds = character.characterData.damagedSounds;
            if (character.characterData.deathSounds != null)
                deathSounds = character.characterData.deathSounds;
        }
    }

    public void PlaySFX(string soundName)
    {
        if (!enableAudio || audioSource == null) return;

        AudioClip clip = GetSFXClip(soundName);
        if (clip != null)
        {
            audioSource.volume = sfxVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayVoice(string voiceName)
    {
        if (!enableAudio || audioSource == null) return;

        AudioClip clip = GetVoiceClip(voiceName);
        if (clip != null)
        {
            audioSource.volume = voiceVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    AudioClip GetSFXClip(string soundName)
    {
        switch (soundName)
        {
            case GameConstants.SFX_DAMAGE:
                return GetRandomClip(damagedSounds);
            case GameConstants.SFX_HEAL:
                return GetRandomClip(healSounds);
            case "Death":
                return GetRandomClip(deathSounds);
            case "Skill":
                return GetRandomClip(skillSounds);
            default:
                return null;
        }
    }

    AudioClip GetVoiceClip(string voiceName)
    {
        switch (voiceName)
        {
            case "Attack":
                return GetRandomClip(attackSounds);
            case "Victory":
                return GetRandomClip(character.characterData?.victoryVoices);
            default:
                return null;
        }
    }

    AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
    }

    public void StopAllAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    // デバッグ用
    [ContextMenu("Play Attack Sound")]
    public void DebugPlayAttackSound()
    {
        PlayVoice("Attack");
    }

    [ContextMenu("Play Damage Sound")]
    public void DebugPlayDamageSound()
    {
        PlaySFX(GameConstants.SFX_DAMAGE);
    }
}