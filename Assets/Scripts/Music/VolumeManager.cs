// Filename: VolumeManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages the audio volume for the game, allowing for persistent volume settings across scenes.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the audio volume in the game, including loading and saving volume settings.
/// </summary>
public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance; // Singleton instance

    public Slider volumeSlider; // Slider UI component for volume control
    private float musicVolume = 1f; // Current music volume
    private AudioSource currentAudioSource; // Reference to the currently playing AudioSource

    /// <summary>
    /// Initializes the singleton instance and prevents destruction on scene load.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set this instance as the singleton
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Loads the saved volume preference and sets the slider value accordingly.
    /// </summary>
    private void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("volume", 1f); // Load the saved volume or default to 1 (max volume)
        if (volumeSlider != null)
        {
            volumeSlider.value = musicVolume; // Set slider value to the current volume
        }
    }

    /// <summary>
    /// Updates the volume of the current audio source in each frame.
    /// </summary>
    private void Update()
    {
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume; // Update the volume of the current audio source
        }
    }

    /// <summary>
    /// Updates the music volume and saves the new volume setting.
    /// </summary>
    /// <param name="volume">The new volume level to set.</param>
    public void UpdateVolume(float volume)
    {
        musicVolume = volume; // Update current volume
        PlayerPrefs.SetFloat("volume", musicVolume); // Save the new volume setting
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume; // Update the volume of the current audio source
        }
    }

    /// <summary>
    /// Sets the current audio source and updates its volume based on the saved settings.
    /// </summary>
    /// <param name="audioSource">The AudioSource to set as the current source.</param>
    public void SetCurrentAudioSource(AudioSource audioSource)
    {
        currentAudioSource = audioSource; // Set the current audio source
        if (currentAudioSource != null)
        {
            currentAudioSource.volume = musicVolume; // Update the volume of the new audio source
        }
    }
}
