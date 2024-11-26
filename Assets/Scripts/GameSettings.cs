using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    [Header("Slider")]
    public Slider musicSlider;
    public Slider soundEffectSlider;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Update()
    {
        musicAudioSource.volume = musicSlider.value;
        soundEffectAudioSource.volume = soundEffectSlider.value;
    }
}
