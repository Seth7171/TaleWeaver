// Filename: HealthBar.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class manages the health bar UI, including setting health values and updating the display.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the health bar UI, including setting health values and updating the display.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Slider slider; // Slider UI component representing health
    public TMP_Text HP_text; // Text component to display health
    public static HealthBar Instance { get; private set; } // Singleton instance

    private void Start()
    {
        // Initialize the HealthBar instance
        Instance = this;

        // Set the initial health if PlayerInGame is available
        if (PlayerInGame.Instance != null)
        {
            SetHealth(PlayerInGame.Instance.currentHealth);
        }
    }

    /// <summary>
    /// Sets the current health value and updates the health display.
    /// </summary>
    /// <param name="health">The current health value to set.</param>
    public void SetHealth(int health)
    {
        slider.value = health; // Update the slider value
        HP_text.text = health.ToString() + "/" + slider.maxValue.ToString(); // Update the health text display
    }

    /// <summary>
    /// Sets the maximum health value and initializes the health display.
    /// </summary>
    /// <param name="health">The maximum health value to set.</param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health; // Set the maximum value of the slider
        slider.value = health; // Initialize the current health to max
        HP_text.text = health.ToString() + "/" + slider.maxValue.ToString(); // Update the health text display
    }
}
