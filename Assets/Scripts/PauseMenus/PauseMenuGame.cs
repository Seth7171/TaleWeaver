using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGame : PauseMenu
{

    public HandBookController handBookController;


    public override void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        handBookController.EnableControls();
        Cursor.visible = false;
        GameIsPaused = false;
    }

    public override void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
        handBookController.DisableControls();
        GameIsPaused = true;
    }
}


