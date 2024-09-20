// Filename: Clear.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages displaying and clearing feedback text in Unity UI.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the display and automatic clearing of feedback text in the UI.
/// </summary>
public class Clear : MonoBehaviour
{
    public float displayDuration = 3f; // Duration to display the feedback text
    public TMP_Text feedbackText; // Reference to the TMP_Text component for feedback display

    /// <summary>
    /// Initiates the feedback clearing process.
    /// </summary>
    public void ClearFeedbackFromUnity()
    {
        StartCoroutine(ClearFeedbackText()); // Start the coroutine to clear feedback text
    }

    /// <summary>
    /// Coroutine that waits for a specified duration and then clears the feedback text.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(displayDuration); // Wait for the specified duration
        feedbackText.text = ""; // Clear the feedback text
    }
}
