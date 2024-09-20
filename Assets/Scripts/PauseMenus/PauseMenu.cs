// Filename: PauseMenu.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages the pause menu functionality, including opening and closing menus, and handling game pause states.

using echo17.EndlessBook.Demo02;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; // Indicates if the game is currently paused
    public static bool SettingsMenuOpen = false; // Indicates if the settings menu is open
    public static bool WarningMenuOpen = false; // Indicates if the warning menu is open

    public GameObject pauseMenuUI; // The main pause menu UI
    public GameObject pauseSettingsMenuUI; // The settings menu UI
    public GameObject pauseMenuMainUI; // The main UI within the pause menu
    public GameObject pauseWarningMenuUI; // The warning menu UI
    public GameObject TouchPad; // TouchPad UI element

    void Start()
    {
        // Ensure initial states
        pauseMenuUI.SetActive(false);
        pauseSettingsMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingsMenuOpen)
            {
                CloseSettingsMenu();
            }
            else if (WarningMenuOpen)
            {
                CloseWarningMenu();
            }
            else if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Resumes the game, hiding the pause menu and resetting time scale.
    /// </summary>
    public virtual void Resume()
    {
        TouchPad.SetActive(true);
        pauseMenuUI.SetActive(false);
        pauseSettingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        SettingsMenuOpen = false;
        WarningMenuOpen = false;
    }

    /// <summary>
    /// Pauses the game, displaying the pause menu and stopping time.
    /// </summary>
    public virtual void Pause()
    {
        TouchPad.SetActive(false);
        pauseMenuUI.SetActive(true);
        pauseSettingsMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        SettingsMenuOpen = false;
        WarningMenuOpen = false;
    }

    /// <summary>
    /// Loads the settings menu.
    /// </summary>
    public virtual void LoadSettingsMenu()
    {
        pauseSettingsMenuUI.SetActive(true);
        pauseMenuMainUI.SetActive(false);
        SettingsMenuOpen = true;
    }

    /// <summary>
    /// Closes the settings menu.
    /// </summary>
    public virtual void CloseSettingsMenu()
    {
        pauseSettingsMenuUI.SetActive(false);
        pauseMenuMainUI.SetActive(true);
        SettingsMenuOpen = false;
    }

    /// <summary>
    /// Loads the warning menu.
    /// </summary>
    public virtual void LoadWarningMenu()
    {
        pauseWarningMenuUI.SetActive(true);
        pauseMenuMainUI.SetActive(false);
        WarningMenuOpen = true;
    }

    /// <summary>
    /// Closes the warning menu.
    /// </summary>
    public virtual void CloseWarningMenu()
    {
        pauseWarningMenuUI.SetActive(false);
        pauseMenuMainUI.SetActive(true);
        WarningMenuOpen = false;
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SettingsMenuOpen = false;
        WarningMenuOpen = false;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        Debug.Log("QUIT...");
        Application.Quit();
    }
}
