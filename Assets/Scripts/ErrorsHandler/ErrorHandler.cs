using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    // Start is called before the first frame update
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


    public void ErrorAccured(string error, string errorCode = "", string errorText = "")
    {
        this.error = error;
        this.errorCode = errorCode;
        this.errorText = errorText;

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Find the parent canvas object
        GameObject parentCanvas = GameObject.Find("Canvas"); // Adjust this to the name of the parent canvas in your scene
        if (parentCanvas != null)
        {
            // Find the child error canvas within the parent canvas
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
