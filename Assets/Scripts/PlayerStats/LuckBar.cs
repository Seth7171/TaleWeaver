// Filename: LuckBar.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class manages the luck bar UI, including setting luck values and updating the display.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the luck bar UI, including setting luck values and updating the display.
/// </summary>
public class LuckBar : MonoBehaviour
{
    public Slider slider; // Slider UI component representing luck
    public TMP_Text Luck_text; // Text component to display luck
    public static LuckBar Instance { get; private set; } // Singleton instance

    private void Start()
    {
        // Initialize the LuckBar instance
        Instance = this;

        // Set the initial luck if PlayerInGame is available
        if (PlayerInGame.Instance != null)
        {
            SetLuck(PlayerInGame.Instance.currentLuck);
        }
    }

    /// <summary>
    /// Sets the current luck value and updates the luck display.
    /// </summary>
    /// <param name="luck">The current luck value to set.</param>
    public void SetLuck(int luck)
    {
        slider.value = luck; // Update the slider value
        Luck_text.text = luck.ToString() + "/" + slider.maxValue.ToString(); // Update the luck text display
    }

    /// <summary>
    /// Sets the maximum luck value and initializes the luck display.
    /// </summary>
    /// <param name="luck">The maximum luck value to set.</param>
    public void SetMaxLuck(int luck)
    {
        slider.maxValue = luck; // Set the maximum value of the slider
        slider.value = luck; // Initialize the current luck to max
        Luck_text.text = luck.ToString() + "/" + slider.maxValue.ToString(); // Update the luck text display
    }
}
