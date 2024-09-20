// Filename: EnsureWordWrapping.cs
// Author: Nitsan Maman & Ron Shahar
// Created on: 15/07/2024
// Description: This class ensures that word wrapping is enabled for specified TextMeshPro components.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Ensures that word wrapping is enabled for specified TextMeshPro components.
/// </summary>
public class EnsureWordWrapping : MonoBehaviour
{
    public TMP_Text textComponent; // The main text component to enable word wrapping
    public TMP_Text placeholderComponent; // The placeholder text component to enable word wrapping

    void Start()
    {
        // Enable word wrapping for the main text component if it's assigned
        if (textComponent != null)
        {
            textComponent.enableWordWrapping = true; // Set word wrapping to true
        }

        // Enable word wrapping for the placeholder text component if it's assigned
        if (placeholderComponent != null)
        {
            placeholderComponent.enableWordWrapping = true; // Set word wrapping to true
        }
    }
}
