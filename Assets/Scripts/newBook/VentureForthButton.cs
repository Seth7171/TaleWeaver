// Filename: VentureForthButton.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages the functionality of the "Venture Forth" button in the game,
// validating input fields, saving player data, and starting a new adventure.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using TaleWeaver.Gameplay;

/// <summary>
/// This class manages the functionality of the Venture Forth button,
/// handling input validation and starting a new adventure.
/// </summary>
public class VentureForthButton : MonoBehaviour
{
    public Button ventureForthButton; // Button for venturing forth
    public TMP_InputField bookNameInput; // Input field for book name
    public TMP_InputField narrativeInput; // Input field for narrative
    public TMP_Text feedbackText; // Text field for feedback messages

    private Player currentPlayer; // Current player instance

    /// <summary>
    /// Loads player data on awake.
    /// </summary>
    void Awake()
    {
        string playerName = PlayerSession.SelectedPlayerName; // Get selected player name
        currentPlayer = DataManager.LoadPlayerData(playerName); // Load player data
    }

    /// <summary>
    /// Handles the Venture Forth action when the button is clicked.
    /// Validates input and saves the new book data.
    /// </summary>
    public void VentureForth()
    {
        string bookName = bookNameInput.text; // Get book name from input
        string narrative = narrativeInput.text; // Get narrative from input

        // Validate book name input
        if (string.IsNullOrEmpty(bookName))
        {
            feedbackText.text = "Book name cannot be empty."; // Set feedback message
            StartCoroutine(ClearFeedbackText()); // Clear message after a delay
            return;
        }

        // Validate narrative input
        if (string.IsNullOrEmpty(narrative))
        {
            feedbackText.text = "Narrative field cannot be empty."; // Set feedback message
            StartCoroutine(ClearFeedbackText()); // Clear message after a delay
            return;
        }

        // Validate allowed characters in narrative
        if (!InputValidator.IsValidInput(narrative))
        {
            feedbackText.text = "Allowed characters are A-Z 0-9 ! ? , ."; // Set feedback message
            StartCoroutine(ClearFeedbackText()); // Clear message after a delay
            return;
        }

        // Check for inappropriate words in narrative
        if (InappropriateWordsFilter.ContainsInappropriateWords(narrative))
        {
            feedbackText.text = "Narrative contains inappropriate content."; // Set feedback message
            StartCoroutine(ClearFeedbackText()); // Clear message after a delay
            return;
        }

        // Save player data and book information
        if (currentPlayer != null)
        {
            currentPlayer.BookNames.Add(bookName); // Add book name to player's list
            DataManager.SavePlayerData(currentPlayer); // Save player data

            Book newBook = new Book(bookName, narrative); // Create new book instance
            DataManager.SaveBookData(currentPlayer.PlayerName, newBook); // Save book data

            Debug.Log("Book saved successfully."); // Log success message
            CreateBookJson(); // Create JSON for the book
        }
        else
        {
            feedbackText.text = "Player not found."; // Set feedback message
            StartCoroutine(ClearFeedbackText()); // Clear message after a delay
        }
        SceneManager.LoadScene("LoadingScene"); // Load loading scene
    }

    /// <summary>
    /// Creates a JSON file for the new book.
    /// </summary>
    private void CreateBookJson()
    {
        if (bookNameInput == null || narrativeInput == null)
        {
            Debug.LogError("Input fields are not assigned."); // Log error if input fields are not assigned
            return;
        }

        string bookName = bookNameInput.text; // Get book name from input
        string narrative = narrativeInput.text; // Get narrative from input

        // Validate inputs
        if (string.IsNullOrEmpty(bookName) || string.IsNullOrEmpty(narrative))
        {
            Debug.LogError("Book name or narrative is empty."); // Log error if inputs are empty
            return;
        }

        // Call the Game Mechanics Manager to start the adventure
        if (GameMechanicsManager.Instance != null)
        {
            GameMechanicsManager.Instance.StartAdventure(bookName, narrative); // Start the adventure with the new book
        }
        else
        {
            Debug.LogError("GameMechanicsManager instance is not initialized."); // Log error if instance is not initialized
        }
    }

    /// <summary>
    /// Clears the feedback text after a specified delay.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(6); // Wait for 6 seconds
        feedbackText.text = ""; // Clear feedback text
    }
}
