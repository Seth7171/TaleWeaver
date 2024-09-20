// Filename: OpenURLButton.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages opening a specified URL in the default web browser when a button is clicked.

using UnityEngine;

/// <summary>
/// Handles opening a URL in the default web browser when a button is clicked.
/// </summary>
public class OpenURLButton : MonoBehaviour
{
    // URL to open
    public string url = "https://platform.openai.com/settings/profile?tab=api-keys";

    /// <summary>
    /// Method to be called when the button is clicked.
    /// Opens the specified URL in the default web browser.
    /// </summary>
    public void OpenURL()
    {
        Application.OpenURL(url); // Open the URL
    }
}
