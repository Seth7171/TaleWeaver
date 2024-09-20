// Filename: ErrorHandler.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Handles errors during gameplay by loading the main menu and displaying an error message on the error canvas.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ErrorHandler manages the process of handling and displaying errors to the user. It loads a specified scene and displays
/// error messages on a designated error canvas.
/// </summary>
public class ErrorHandler : MonoBehaviour
{
    public static ErrorHandler Instance { get; private set; }

    public GameObject errorCanvas;
    public TextMeshProUGUI errorLog;
    public string errorCanvasName = "Error Alert"; // Name of the errorCanvas in the new scene
    public string errorLogName = "Error log"; // Name of the errorLog object

    private string error;
    private string errorCode;
    private string errorText;

    /// <summary>
    /// Initializes the ErrorHandler as a singleton and persists it across scenes.
    /// </summary>
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ErrorHandler instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles the error by recording the error details and loading the MainMenu scene to display the error.
    /// </summary>
    /// <param name="error">The error message to be displayed.</param>
    /// <param name="errorCode">The optional error code associated with the error.</param>
    /// <param name="errorText">Additional error text or details.</param>
    public void ErrorAccured(string error, string errorCode = "", string errorText = "")
    {
        this.error = error;
        this.errorCode = errorCode;
        this.errorText = errorText;

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Called when a scene has been loaded, setting up the error canvas to display the error.
    /// </summary>
    /// <param name="scene">The loaded scene.</param>
    /// <param name="mode">The scene load mode.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
        StartCoroutine(WaitForCanvas()); // Start a coroutine to wait for the canvas to fully load
    }

    /// <summary>
    /// Waits for one frame to ensure that the canvas is fully loaded and then displays the error message.
    /// </summary>
    private IEnumerator WaitForCanvas()
    {
        yield return null; // Wait for one frame

        // Attempt to find the parent canvas object
        GameObject parentCanvas = GameObject.Find("Canvas");
        if (parentCanvas != null)
        {
            // Find the error canvas within the parent canvas
            Transform errorCanvasTransform = parentCanvas.transform.Find(errorCanvasName);
            if (errorCanvasTransform != null)
            {
                GameObject errorCanvas = errorCanvasTransform.gameObject;
                TextMeshProUGUI errorLog = errorCanvasTransform.Find(errorLogName).GetComponent<TextMeshProUGUI>();
                if (errorLog != null)
                {
                    errorCanvas.SetActive(true);
                    errorLog.text = error + "\n" + errorCode + "\n" + errorText + "\n";
                }
                else
                {
                    Debug.LogError("Error text object not found!");
                }
            }
            else
            {
                Debug.LogError("Error canvas not found within the parent canvas!");
            }
        }
        else
        {
            Debug.LogError("Parent canvas not found!");
        }

        // Clear the error information after displaying
        this.error = null;
        this.errorCode = null;
        this.errorText = null;
    }
}
