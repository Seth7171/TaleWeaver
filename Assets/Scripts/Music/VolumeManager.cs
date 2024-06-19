using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    public Slider volumeSlider;
    private float musicVolume = 1f;
    private AudioSource currentAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load the saved volume
        musicVolume = PlayerPrefs.GetFloat("volume", 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = musicVolume;
        }
    }

    private void Update()
    {
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume;
        }
    }

    public void UpdateVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("volume", musicVolume);
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume;
        }
    }

    public void SetCurrentAudioSource(AudioSource audioSource)
    {
        currentAudioSource = audioSource;
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume;
        }
    }
}
