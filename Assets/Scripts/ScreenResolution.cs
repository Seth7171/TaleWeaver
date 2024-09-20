// Filename: SetResolution.cs
// Author: Nitsan Maman & Ron Shahar
// Created on: 15/07/2024
// Description: This class sets the game resolution at the start of the scene.

using UnityEngine;

/// <summary>
/// Sets the game resolution at the start of the scene.
/// </summary>
public class SetResolution : MonoBehaviour
{
    void Start()
    {
        // Set the resolution to 1920x1080 (Full HD) in fullscreen window mode
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
