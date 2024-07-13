using echo17.EndlessBook.Demo02;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool SettingsMenuOpen = false;

    public GameObject pauseMenuUI;
    public GameObject pauseSettingsMenuUI;
    public GameObject pauseMenuMainUI;
    public GameObject TouchPad;

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

    public virtual void Resume()
    {
        TouchPad.SetActive(true);
        pauseMenuUI.SetActive(false);
        pauseSettingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        SettingsMenuOpen = false;
    }

    public virtual void Pause()
    {
        TouchPad.SetActive(false);
        pauseMenuUI.SetActive(true);
        pauseSettingsMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        SettingsMenuOpen = false;
    }

    public virtual void LoadSettingsMenu()
    {
        pauseSettingsMenuUI.SetActive(true);
        pauseMenuMainUI.SetActive(false);
        SettingsMenuOpen = true;
    }

    public virtual void CloseSettingsMenu()
    {
        pauseSettingsMenuUI.SetActive(false);
        pauseMenuMainUI.SetActive(true);
        SettingsMenuOpen = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SettingsMenuOpen = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Debug.Log("QUIT...");
        Application.Quit();
    }
}
