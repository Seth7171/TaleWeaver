// Filename: ModifierNum.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class manages the display of skill modifiers in the UI, updating the text and color based on the modifier value.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the display of skill modifiers in the UI, updating the text and color based on the modifier value.
/// </summary>
public class ModifierNum : MonoBehaviour
{
    //public Slider slider; // Slider UI component (commented out for future use)
    public TMP_Text Check_Modifier_text; // Text component to display the skill modifier
    private Color lessBrightGreen = new Color(0.0f, 0.5f, 0.0f); // Color for negative modifiers
    public static ModifierNum Instance { get; private set; } // Singleton instance

    private void Start()
    {
        // Initialize the ModifierNum instance
        Instance = this;

        // Set the initial modifier if PlayerInGame is available
        if (PlayerInGame.Instance != null)
        {
            SetCheckModifier(PlayerInGame.Instance.currentSkillModifier);
        }
    }

    /// <summary>
    /// Sets the displayed skill modifier and updates the text color based on its value.
    /// </summary>
    /// <param name="modifier">The skill modifier value to display.</param>
    public void SetCheckModifier(int modifier)
    {
        if (modifier > 0)
        {
            Check_Modifier_text.color = Color.red; // Set text color to red for positive modifiers
            Check_Modifier_text.text = "+" + modifier.ToString(); // Display positive modifier with a plus sign
        }
        else if (modifier < 0)
        {
            Check_Modifier_text.color = lessBrightGreen; // Set text color to less bright green for negative modifiers
            Check_Modifier_text.text = modifier.ToString(); // Display negative modifier
        }
        else
        {
            Check_Modifier_text.color = Color.black; // Set text color to black for zero modifier
            Check_Modifier_text.text = modifier.ToString(); // Display zero modifier
        }
    }
}
