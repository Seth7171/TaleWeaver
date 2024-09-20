// Filename: MainMenu.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages the main menu of the game, allowing users to select models and start the game.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the main menu, including model selection and game start functionality.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuBackground; // Background of the main menu
    public TMP_Text DALLEModelLabel; // Label for the selected DALL-E model
    public TMP_Dropdown DALLEModelDropDown; // Dropdown for selecting DALL-E models

    /// <summary>
    /// Sets the DALL-E model based on the selected label and updates the dropdown.
    /// </summary>
    public void SetDALLEModel()
    {
        // Determine the selected DALL-E model and set corresponding values
        switch (DALLEModelLabel.text)
        {
            case "dall-e-3 (1024x1024) expensive":
                OpenAIInterface.Instance.current_model = "dall-e-3";
                OpenAIInterface.Instance.current_size = "1024x1024";
                DALLEModelDropDown.value = 0; // Set dropdown to first option
                break;
            case "dall-e-2 (1024x1024) ideal":
                OpenAIInterface.Instance.current_model = "dall-e-2";
                OpenAIInterface.Instance.current_size = "1024x1024";
                DALLEModelDropDown.value = 1; // Set dropdown to second option
                break;
            case "dall-e-2 (512x512) cheap":
                OpenAIInterface.Instance.current_model = "dall-e-2";
                OpenAIInterface.Instance.current_size = "512x512";
                DALLEModelDropDown.value = 2; // Set dropdown to third option
                break;
        }

        // Save the model info in PlayerPrefs
        PlayerPrefs.SetString("DALLEModel", DALLEModelLabel.text); // Save selected model
        PlayerPrefs.Save(); // Ensure changes are saved
        DALLEModelDropDown.RefreshShownValue(); // Refresh dropdown display
    }

    /// <summary>
    /// Initializes the menu by loading the selected model from PlayerPrefs.
    /// </summary>
    private void Awake()
    {
        // Load the model info from PlayerPrefs
        DALLEModelLabel.text = PlayerPrefs.GetString("DALLEModel", "dall-e-2 (1024x1024) ideal"); // Default model if not set
        SetDALLEModel(); // Set model based on loaded value
    }

    /// <summary>
    /// Starts the game by loading the next scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        Debug.Log("QUIT!"); // Log quit action
        Application.Quit(); // Close the application
    }
}
