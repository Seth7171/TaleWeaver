using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGame : PauseMenu
{

    public HandBookController handBookController;
    public GameObject decisions_Canvas;
    public GameObject deathCanvas;

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

    public override void Resume()
    {
        decisions_Canvas.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        handBookController.EnableControls();
        GameIsPaused = false;
        SettingsMenuOpen = false;
    }

    public override void Pause()
    {
        if (deathCanvas.activeSelf == false)
        {
            decisions_Canvas.SetActive(false);
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
            handBookController.DisableControls();
            GameIsPaused = true;
            pauseSettingsMenuUI.SetActive(false);
            SettingsMenuOpen = false;
        }
    }
}


