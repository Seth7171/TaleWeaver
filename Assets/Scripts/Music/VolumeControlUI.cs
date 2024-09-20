// Filename: VolumeControlUI.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages the volume control UI, allowing users to adjust the game's audio volume.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the volume control UI, allowing users to adjust the audio volume using a slider.
/// </summary>
public class VolumeControlUI : MonoBehaviour
{
    public Slider volumeSlider; // Slider UI component for volume control

    /// <summary>
    /// Initializes the volume slider and sets up the listener for volume changes.
    /// </summary>
    private void Start()
    {
        // Set the slider value based on saved volume preference
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f); // Default to 1 (max volume)

        // Add a listener to update the volume when the slider value changes
        volumeSlider.onValueChanged.AddListener(delegate { VolumeManager.Instance.UpdateVolume(volumeSlider.value); });
    }
}
