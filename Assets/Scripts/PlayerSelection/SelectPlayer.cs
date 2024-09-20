// Filename: SelectPlayer.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages player selection, creation, and navigation for starting adventures in the game.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles player selection, creation, and management of adventure states.
/// </summary>
public class SelectPlayer : MonoBehaviour
{
    // UI elements for player management
    public TMP_Text playerNameText; // Displays the current player's name
    public TMP_Text feedbackText; // Displays feedback messages
    public TMP_InputField playerNameInput; // Input field for new player name
    public TMP_InputField apiKeyInput; // Input field for new player API key
    public TMP_InputField UpdatedapiKeyInput; // Input field for updating API key
    public GameObject newPlayerMenu; // UI panel for creating a new player
    public GameObject PlayerMenu; // Main player selection menu
    public GameObject previousAdventuresWindow; // Window to display previous adventures
    public GameObject scrollViewContent; // Content area for previous adventure buttons
    public GameObject bookButtonPrefab; // Prefab for the book buttons
    public Button startNewAdventureButton; // Button to start a new adventure
    public Button viewPreviousAdventuresButton; // Button to view previous adventures
    public Button editAPIkeyButton; // Button to edit the API key
    public Button deletePlayerButton; // Button to delete a player
    public Button leftArrowButton; // Button to navigate left through players
    public Button rightArrowButton; // Button to navigate right through players
    public Button StartPrevAdventure; // Button to start a previous adventure
    public Button DeletePrevAdventure; // Button to delete a previous adventure
    public float displayDuration = 3f; // Duration for displaying feedback messages

    private PlayerManager playerManager; // Manager for handling player data
    private List<string> players; // List of player names
    private int currentPlayerIndex = -1; // Index of the currently selected player
    private TMP_Text selectedBookButtonText = null; // Reference to the currently selected book button text

    /// <summary>
    /// Initializes the player selection menu.
    /// </summary>
    void Start()
    {
        StartPrevAdventure.interactable = false;
        DeletePrevAdventure.interactable = false;
        playerManager = DataManager.LoadPlayerManager();
        players = playerManager.PlayerNames;

        // Check if there are any players available
        if (players.Count > 0)
        {
            currentPlayerIndex = 0; // Set to the first player
            DisplayCurrentPlayer(); // Display the selected player's information
        }
        else
        {
            // Disable buttons if no players are found
            startNewAdventureButton.interactable = false;
            viewPreviousAdventuresButton.interactable = false;
            deletePlayerButton.interactable = false;
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
            editAPIkeyButton.interactable = false;
        }
    }

    /// <summary>
    /// Displays the currently selected player.
    /// </summary>
    void DisplayCurrentPlayer()
    {
        // Check if the current player index is valid
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            playerNameText.text = players[currentPlayerIndex]; // Show player name
            // Enable buttons for interaction
            startNewAdventureButton.interactable = true;
            viewPreviousAdventuresButton.interactable = true;
            deletePlayerButton.interactable = true;
            leftArrowButton.interactable = true;
            rightArrowButton.interactable = true;
            editAPIkeyButton.interactable = true;
        }
        else
        {
            // Reset UI if no valid player is selected
            playerNameText.text = "";
            startNewAdventureButton.interactable = false;
            viewPreviousAdventuresButton.interactable = false;
            deletePlayerButton.interactable = false;
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
            editAPIkeyButton.interactable = false;
        }
    }

    /// <summary>
    /// Starts a new adventure for the selected player.
    /// </summary>
    public void StartNewAdventure()
    {
        // Ensure a player is selected
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string selectedPlayer = players[currentPlayerIndex];
            Player player = DataManager.LoadPlayerData(selectedPlayer); // Load the selected player's data

            if (player != null)
            {
                // Set session data for the selected player
                PlayerSession.SelectedPlayerName = player.PlayerName;
                PlayerSession.SelectedPlayerApiKey = player.ApiKey;
                PlayerSession.SelectedPlayerassistantID = player.assistantID;

                // Initialize OpenAI API keys
                if (OpenAIInterface.Instance != null)
                {
                    OpenAIInterface.Instance.LoadPlayerAPIKeys();
                }
                else
                {
                    Debug.LogError("OpenAIInterface instance is not initialized.");
                }

                // Load the next scene for the adventure
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                feedbackText.text = "Failed to load player data."; // Handle error
            }
        }
        else
        {
            feedbackText.text = "Please select a player first."; // Prompt for player selection
        }
        StartCoroutine(ClearFeedbackText()); // Clear feedback after a duration
    }

    /// <summary>
    /// Initiates the process to create an assistant with the given API key.
    /// </summary>
    /// <param name="apiKey">The API key to use for the assistant.</param>
    /// <param name="flag">Flag indicating whether to create a new assistant (0) or update an existing one (1).</param>
    public void StartCreateAssistant(string apiKey, int flag)
    {
        if (flag == 0)
        {
            // Call the OpenAI Interface to create a new assistant
            if (OpenAIInterface.Instance != null)
            {
                StartCoroutine(OpenAIInterface.Instance.CreateAPI_Assistant(apiKey, CreatePlayer_Part2));
            }
            else
            {
                Debug.LogError("OpenAIInterface instance is not initialized.");
            }
        }
        if (flag == 1)
        {
            // Call the OpenAI Interface to update an existing assistant
            if (OpenAIInterface.Instance != null)
            {
                StartCoroutine(OpenAIInterface.Instance.CreateAPI_Assistant(apiKey, UpdateAPIkey_Part2));
            }
            else
            {
                Debug.LogError("OpenAIInterface instance is not initialized.");
            }
        }
    }

    /// <summary>
    /// Creates a new player with the specified name and API key.
    /// </summary>
    public void CreatePlayer()
    {
        string playerName = playerNameInput.text; // Get player name from input
        string apiKey = apiKeyInput.text; // Get API key from input

        // Validate input fields
        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(apiKey))
        {
            feedbackText.text = "Name and API key cannot be empty.";
            StartCoroutine(ClearFeedbackText());
            return;
        }

        Player newPlayer = new Player(playerName, apiKey, ""); // Create a new player object
        bool playerAdded = playerManager.AddPlayer(playerName); // Attempt to add the player

        if (playerAdded)
        {
            DataManager.CreatePlayerFolder(playerName); // Create folder for the player
            DataManager.SavePlayerManager(playerManager); // Save player manager data
            DataManager.SavePlayerData(newPlayer); // Save new player data

            StartCreateAssistant(apiKey, 0); // Start assistant creation
        }
        else
        {
            feedbackText.text = "Player with this name already exists."; // Handle duplicate player
            StartCoroutine(ClearFeedbackText());
        }
    }

    /// <summary>
    /// Handles actions after creating a new player.
    /// </summary>
    /// <param name="success">Indicates whether the assistant creation was successful.</param>
    /// <param name="apiKey">The API key used for the assistant.</param>
    private void CreatePlayer_Part2(bool success, string apiKey)
    {
        string playerName = playerNameInput.text; // Get the player name from input
        if (success)
        {
            Player player = DataManager.LoadPlayerData(playerName); // Load player data
            player.assistantID = OpenAIInterface.Instance.AssistantID; // Set assistant ID
            DataManager.SavePlayerData(player); // Save updated player data
            Debug.Log("Assistant creation succeeded.");
            feedbackText.text = "Player created successfully.";
            players = playerManager.PlayerNames; // Refresh player list
            currentPlayerIndex = players.Count - 1; // Set to the new player
            DisplayCurrentPlayer(); // Display the new player
            playerNameInput.text = ""; // Clear input fields
            apiKeyInput.text = "";
            newPlayerMenu.SetActive(false); // Hide new player menu
            PlayerMenu.SetActive(true); // Show player menu
            StartCoroutine(ClearFeedbackText()); // Clear feedback after a duration
        }
        else
        {
            bool playerDeleted = playerManager.DeletePlayer(playerName); // Attempt to delete the player if creation fails

            if (playerDeleted)
            {
                DataManager.DeletePlayerFolder(playerName); // Delete player folder
                DataManager.SavePlayerManager(playerManager); // Save player manager data

                feedbackText.text = "Player deleted successfully."; // Notify user
                players = playerManager.PlayerNames; // Refresh player list
                currentPlayerIndex = players.Count > 0 ? 0 : -1; // Reset current player index
                DisplayCurrentPlayer(); // Display the updated player
                StartCoroutine(ClearFeedbackText()); // Clear feedback after a duration
            }
            else
            {
                feedbackText.text = "Player not found."; // Handle player not found error
            }
        }
    }

    /// <summary>
    /// Clears input fields when navigating back.
    /// </summary>
    public void ClearPlayerTextOnBack()
    {
        playerNameInput.text = ""; // Clear player name input
        apiKeyInput.text = ""; // Clear API key input
    }

    /// <summary>
    /// Updates the API key for the currently selected player.
    /// </summary>
    public void UpdateAPIkey()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string playerName = players[currentPlayerIndex]; // Get the current player's name
            string newApiKey = UpdatedapiKeyInput.text; // Get the new API key

            // Validate API key input
            if (string.IsNullOrEmpty(newApiKey))
            {
                feedbackText.text = "API key cannot be empty.";
                StartCoroutine(ClearFeedbackText());
                return;
            }

            Player player = DataManager.LoadPlayerData(playerName); // Load player data
            if (player != null)
            {
                StartCreateAssistant(newApiKey, 1); // Start API key update
            }
            else
            {
                feedbackText.text = "Failed to load player data."; // Handle error
            }
        }
        else
        {
            feedbackText.text = "No player selected."; // Prompt for player selection
        }
        StartCoroutine(ClearFeedbackText()); // Clear feedback after a duration
    }

    /// <summary>
    /// Handles actions after updating the API key.
    /// </summary>
    /// <param name="success">Indicates whether the API key update was successful.</param>
    /// <param name="apiKey">The new API key.</param>
    private void UpdateAPIkey_Part2(bool success, string apiKey)
    {
        string playerName = players[currentPlayerIndex]; // Get the current player's name
        if (success)
        {
            Player player = DataManager.LoadPlayerData(playerName); // Load player data
            player.assistantID = OpenAIInterface.Instance.AssistantID; // Set the assistant ID
            player.ApiKey = apiKey; // Update API key
            DataManager.SavePlayerData(player); // Save updated player data
            feedbackText.text = "API key updated successfully."; // Notify user
            UpdatedapiKeyInput.text = ""; // Clear the API key input
        }
        else
        {
            feedbackText.text = "API key was not updated!"; // Handle update failure
        }
    }

    /// <summary>
    /// Deletes the currently selected player.
    /// </summary>
    public void DeletePlayer()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string playerName = players[currentPlayerIndex]; // Get the current player's name
            bool playerDeleted = playerManager.DeletePlayer(playerName); // Attempt to delete the player

            if (playerDeleted)
            {
                DataManager.DeletePlayerFolder(playerName); // Delete player folder
                DataManager.SavePlayerManager(playerManager); // Save player manager data

                feedbackText.text = "Player deleted successfully."; // Notify user
                players = playerManager.PlayerNames; // Refresh player list
                currentPlayerIndex = players.Count > 0 ? 0 : -1; // Reset current player index
                DisplayCurrentPlayer(); // Display the updated player
                StartCoroutine(ClearFeedbackText()); // Clear feedback after a duration
            }
            else
            {
                feedbackText.text = "Player not found."; // Handle player not found error
                StartCoroutine(ClearFeedbackText());
            }
        }
        else
        {
            feedbackText.text = "No player selected."; // Prompt for player selection
            StartCoroutine(ClearFeedbackText());
        }
    }

    /// <summary>
    /// Shows the new player creation menu.
    /// </summary>
    public void ShowNewPlayerMenu()
    {
        newPlayerMenu.SetActive(true); // Activate new player menu
    }

    /// <summary>
    /// Hides the new player creation menu.
    /// </summary>
    public void HideNewPlayerMenu()
    {
        newPlayerMenu.SetActive(false); // Deactivate new player menu
    }

    /// <summary>
    /// Navigates to the previous player in the list.
    /// </summary>
    public void NavigateLeft()
    {
        if (players.Count > 0)
        {
            currentPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count; // Cycle left through players
            DisplayCurrentPlayer(); // Update displayed player
        }
    }

    /// <summary>
    /// Navigates to the next player in the list.
    /// </summary>
    public void NavigateRight()
    {
        if (players.Count > 0)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count; // Cycle right through players
            DisplayCurrentPlayer(); // Update displayed player
        }
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }

    /// <summary>
    /// Shows the previous adventures for the selected player.
    /// </summary>
    public void ShowPreviousAdventures()
    {
        Debug.Log("ShowPreviousAdventures called");

        // Clear existing buttons in the scroll view content
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        if (this.currentPlayerIndex >= 0 && this.currentPlayerIndex < players.Count)
        {
            string selectedPlayer = playerNameText.text; // Get the currently selected player's name
            Player player = DataManager.LoadPlayerData(selectedPlayer); // Load player data

            if (player != null)
            {
                // Create buttons for each book in the player's list
                foreach (string bookName in player.BookNames)
                {
                    Debug.Log($"Creating button for book: {bookName}");
                    GameObject button = Instantiate(bookButtonPrefab, scrollViewContent.transform); // Instantiate button prefab
                    button.SetActive(true); // Activate the button
                    TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(); // Get the text component
                    if (buttonText != null)
                    {
                        buttonText.text = bookName; // Set button text
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component not found on button prefab.");
                    }

                    Button btnComponent = button.GetComponent<Button>(); // Get the button component
                    if (btnComponent != null)
                    {
                        btnComponent.onClick.AddListener(() => OnBookButtonClicked(selectedPlayer, bookName, btnComponent, buttonText)); // Add click listener
                    }
                    else
                    {
                        Debug.LogError("Button component not found on button prefab.");
                    }
                }
            }
            else
            {
                feedbackText.text = "Failed to load player data."; // Handle error
                StartCoroutine(ClearFeedbackText());
            }
        }
        else
        {
            feedbackText.text = "Please select a player first."; // Prompt for player selection
            StartCoroutine(ClearFeedbackText());
        }

        previousAdventuresWindow.SetActive(true); // Show previous adventures window
    }

    /// <summary>
    /// Handles the click event for a book button.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="bookName">The name of the book.</param>
    /// <param name="button">The button that was clicked.</param>
    /// <param name="buttonText">The text component of the button.</param>
    private void OnBookButtonClicked(string playerName, string bookName, Button button, TMP_Text buttonText)
    {
        Debug.Log($"Button clicked for book: {bookName}");

        // Reset the color of the previously selected button's text
        if (selectedBookButtonText != null)
        {
            selectedBookButtonText.color = new Color(225 / 255f, 209 / 255f, 134 / 255f); // Default text color
        }

        // Set the color of the newly selected button's text
        buttonText.color = new Color(123 / 255f, 40 / 255f, 7 / 255f); // Selected text color

        // Update the selected button text reference
        selectedBookButtonText = buttonText;

        PlayerSession.SelectedPlayerName = playerName; // Set the selected player
        PlayerSession.SelectedBookName = bookName; // Set the selected book
        StartPrevAdventure.interactable = true; // Enable the start previous adventure button
        DeletePrevAdventure.interactable = true; // Enable the delete previous adventure button

        // Pass the playerName and bookName to the DeletePrevAdventureButtonClicked method
        DeletePrevAdventure.onClick.RemoveAllListeners(); // Clear any previous listeners
        DeletePrevAdventure.onClick.AddListener(() => DeletePrevAdventureButtonClicked(playerName, bookName)); // Add new listener
    }

    /// <summary>
    /// Starts the previous adventure for the selected player and book.
    /// </summary>
    public void StartPrevAdventureButtonClicked()
    {
        StartPrevAdventure.interactable = false; // Disable the button
        DeletePrevAdventure.interactable = false; // Disable the delete button
        SceneManager.LoadScene("ViewPrevAdv"); // Load the previous adventure scene
    }

    /// <summary>
    /// Deletes the selected previous adventure.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="bookName">The name of the book to delete.</param>
    private void DeletePrevAdventureButtonClicked(string playerName, string bookName)
    {
        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(bookName))
        {
            Player player = DataManager.LoadPlayerData(playerName); // Load player data
            if (player != null && player.BookNames.Contains(bookName))
            {
                // Remove the book from the player's list
                player.RemoveBook(bookName);
                // Save the updated player data
                DataManager.SavePlayerData(player);

                // Delete the book data from the filesystem
                DataManager.DeleteBookData(playerName, bookName);

                feedbackText.text = $"Book '{bookName}' deleted successfully."; // Notify user

                // Optionally, update the previous adventures window
                ShowPreviousAdventures();
            }
            else
            {
                feedbackText.text = "Book not found in player data."; // Handle book not found error
                StartCoroutine(ClearFeedbackText());
            }
        }
        else
        {
            feedbackText.text = "No book selected to delete."; // Prompt for book selection
            StartCoroutine(ClearFeedbackText());
        }

        StartPrevAdventure.interactable = false; // Disable the start button
        DeletePrevAdventure.interactable = false; // Disable the delete button
    }

    /// <summary>
    /// Clears feedback messages from the UI.
    /// </summary>
    public void ClearFeedbackFromUnity()
    {
        StartCoroutine(ClearFeedbackText());
    }

    /// <summary>
    /// Coroutine to clear feedback text after a specified duration.
    /// </summary>
    /// <returns>Wait for the specified duration.</returns>
    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackText.text = ""; // Clear the feedback text
    }
}
