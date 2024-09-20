// Filename: Player1.cs
// Author: Nitsan Maman & Ron Shahar
// Created on: 15/07/2024
// Description: This singleton script manages the player instance, ensuring it persists across scenes and is destroyed when returning to the main menu.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player1 : MonoBehaviour
{
    public static Player1 Instance { get; private set; } // Singleton instance

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("Player1 instance is being destroyed."); // Log destruction of the instance
            Instance = null; // Clear the singleton instance
        }
    }

    /// <summary>
    /// Initializes the singleton instance and prevents destruction on scene load.
    /// </summary>
    private void Start()
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
    /// Subscribes to the sceneLoaded event.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Handle scene loading events
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event to prevent memory leaks.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up event subscription
    }

    /// <summary>
    /// Handles the actions that occur when a new scene is loaded.
    /// </summary>
    /// <param name="scene">The scene that has been loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject); // Destroy the Player1 instance when returning to the main menu
        }
    }
}
