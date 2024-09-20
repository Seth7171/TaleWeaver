// Filename: PauseMenuGame.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Extends the PauseMenu functionality to include environment control and decision canvases, handling game pause states and UI interactions.

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the pause menu functionality specific to gameplay, including environment settings and decision handling.
/// </summary>
public class PauseMenuGame : PauseMenu
{
    public HandBookController handBookController; // Reference to the HandBookController
    public GameObject decisions_Canvas; // The decisions canvas UI
    public GameObject deathCanvas; // The death screen canvas UI
    public GameObject enviroCanvas; // The environment settings canvas UI
    public bool enviroOpen = false; // Indicates if the environment menu is open

    public static PauseMenuGame Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject); // Destroy this object if the main menu is loaded
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Handle pause menu navigation
            if (enviroOpen)
            {
                BackEnviro(); // Close the environment menu
            }
            else if (SettingsMenuOpen)
            {
                CloseSettingsMenu(); // Close the settings menu
            }
            else if (GameIsPaused)
            {
                Resume(); // Resume the game
            }
            else
            {
                Pause(); // Pause the game
            }
        }
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    public override void Resume()
    {
        decisions_Canvas.SetActive(true); // Show decisions canvas
        pauseMenuUI.SetActive(false); // Hide pause menu UI
        Time.timeScale = 1f; // Resume game time
        Cursor.visible = false; // Hide cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor
        handBookController.EnableControls(); // Enable controls
        handBookController.is_scroll_lock = false; // Allow scrolling
        GameIsPaused = false; // Update pause state
        SettingsMenuOpen = false; // Update settings state
    }

    /// <summary>
    /// Pauses the game and shows the pause menu.
    /// </summary>
    public override void Pause()
    {
        if (!deathCanvas.activeSelf)
        {
            decisions_Canvas.SetActive(false); // Hide decisions canvas
            pauseMenuUI.SetActive(true); // Show pause menu UI
            Time.timeScale = 0f; // Stop game time
            Cursor.visible = true; // Show cursor
            Cursor.lockState = CursorLockMode.None; // Free cursor
            handBookController.DisableControls(); // Disable controls
            handBookController.is_scroll_lock = true; // Lock scrolling
            GameIsPaused = true; // Update pause state
            pauseSettingsMenuUI.SetActive(false); // Ensure settings menu is closed
            SettingsMenuOpen = false; // Update settings state
        }
    }

    /// <summary>
    /// Opens the environment settings menu.
    /// </summary>
    public void Enviro()
    {
        enviroOpen = true; // Set environment menu state
        pauseSettingsMenuUI.SetActive(false); // Hide settings menu
        enviroCanvas.SetActive(true); // Show environment canvas
        Time.timeScale = 1f; // Resume game time
    }

    /// <summary>
    /// Closes the environment settings menu and returns to the settings menu.
    /// </summary>
    public void BackEnviro()
    {
        enviroOpen = false; // Reset environment menu state
        enviroCanvas.SetActive(false); // Hide environment canvas
        pauseSettingsMenuUI.SetActive(true); // Show settings menu
        Time.timeScale = 0; // Pause game time
    }
}
