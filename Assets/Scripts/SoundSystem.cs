using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string tag;
    public AudioClip sound;

    public enum SoundType
    {
        SoundEffect,
        Music
    }
    public SoundType soundType;
}

public class SoundSystem : MonoBehaviour
{
    public static SoundSystem Instance { get; private set; }

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    [Header("Sounds")]
    [SerializeField] private List<Sound> sounds = new();
    private Dictionary<string, Sound> soundDictionary = new();

    private void Start()
    {
        if (!Instance)
            Instance = this;

        soundDictionary = sounds.ToDictionary(key => key.tag, element => element);
    }

    public void PlaySound(string soundTag)
    {
        switch (soundDictionary[soundTag].soundType)
        {
            case Sound.SoundType.SoundEffect:
                soundEffectAudioSource.clip = soundDictionary[soundTag].sound;
                soundEffectAudioSource.Play();
                break;
            case Sound.SoundType.Music:
                musicAudioSource.clip = soundDictionary[soundTag].sound;
                musicAudioSource.Play();
                break;
        }
    }
}
