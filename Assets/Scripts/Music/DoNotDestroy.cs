// Filename: DoNotDestroy.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script ensures that the GameObject it is attached to persists across scene loads, managing audio settings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages a GameObject that should not be destroyed when loading new scenes,
/// specifically for handling background music and audio settings.
/// </summary>
public class DoNotDestroy : MonoBehaviour
{
    private AudioSource audioSource; // Reference to the AudioSource component
    public static DoNotDestroy Instance { get; private set; } // Singleton instance

    /// <summary>
    /// Initializes the singleton instance and prevents destruction on scene load.
    /// </summary>
    private void Awake()
    {
        // GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (Instance == null)
        {
            Instance = this; // Set this instance as the singleton
            DontDestroyOnLoad(this.gameObject); // Prevent this object from being destroyed on scene load
            audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
            VolumeManager.Instance?.SetCurrentAudioSource(audioSource); // Set the current audio source
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Handle scene loading events
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up event subscription
    }

    /// <summary>
    /// Handles actions when a new scene is loaded, destroying the object if in a non-specified scene.
    /// </summary>
    /// <param name="scene">The scene that has been loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy this GameObject if not in specified scenes
        if (scene.name != "MainMenu" && scene.name != "PlayerSelection" && scene.name != "NewBook")
        {
            Destroy(this.gameObject);
        }
        else
        {
            VolumeManager.Instance?.SetCurrentAudioSource(audioSource);
        }
    }
}