using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectPlayer : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text feedbackText;
    public TMP_InputField playerNameInput;
    public TMP_InputField apiKeyInput;
    public TMP_InputField UpdatedapiKeyInput;
    public GameObject newPlayerMenu;
    public GameObject previousAdventuresWindow;
    public GameObject scrollViewContent;
    public GameObject bookButtonPrefab;
    public Button startNewAdventureButton;
    public Button viewPreviousAdventuresButton;
    public Button editAPIkeyButton;
    public Button deletePlayerButton;
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button StartPrevAdventure;
    public Button DeletePrevAdventure;
    public float displayDuration = 3f;

    private PlayerManager playerManager;
    private List<string> players;
    private int currentPlayerIndex = -1;
    private TMP_Text selectedBookButtonText = null;

    void Start()
    {
        StartPrevAdventure.interactable = false;
        DeletePrevAdventure.interactable = false;
        playerManager = DataManager.LoadPlayerManager();
        players = playerManager.PlayerNames;

        if (players.Count > 0)
        {
            currentPlayerIndex = 0;
            DisplayCurrentPlayer();
        }
        else
        {
            startNewAdventureButton.interactable = false;
            viewPreviousAdventuresButton.interactable = false;
            deletePlayerButton.interactable = false;
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
            editAPIkeyButton.interactable = false;
        }
    }

    void DisplayCurrentPlayer()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            playerNameText.text = players[currentPlayerIndex];
            startNewAdventureButton.interactable = true;
            viewPreviousAdventuresButton.interactable = true;
            deletePlayerButton.interactable = true;
            leftArrowButton.interactable = true;
            rightArrowButton.interactable = true;
            editAPIkeyButton.interactable = true;
        }
        else
        {
            playerNameText.text = "";
            startNewAdventureButton.interactable = false;
            viewPreviousAdventuresButton.interactable = false;
            deletePlayerButton.interactable = false;
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
            editAPIkeyButton.interactable = false;
        }
    }

    public void StartNewAdventure()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string selectedPlayer = players[currentPlayerIndex];
            Player player = DataManager.LoadPlayerData(selectedPlayer);

            if (player != null)
            {
                PlayerSession.SelectedPlayerName = player.PlayerName;
                PlayerSession.SelectedPlayerApiKey = player.ApiKey;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                feedbackText.text = "Failed to load player data.";
            }
        }
        else
        {
            feedbackText.text = "Please select a player first.";
        }
    }

    public void CreatePlayer()
    {
        string playerName = playerNameInput.text;
        string apiKey = apiKeyInput.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(apiKey))
        {
            feedbackText.text = "Name and API key cannot be empty.";
            return;
        }

        Player newPlayer = new Player(playerName, apiKey);
        bool playerAdded = playerManager.AddPlayer(playerName);

        if (playerAdded)
        {
            DataManager.CreatePlayerFolder(playerName);
            DataManager.SavePlayerManager(playerManager);
            DataManager.SavePlayerData(newPlayer);

            feedbackText.text = "Player created successfully.";
            players = playerManager.PlayerNames;
            currentPlayerIndex = players.Count - 1;
            DisplayCurrentPlayer();
            playerNameInput.text = "";
            apiKeyInput.text = "";
            newPlayerMenu.SetActive(false);
        }
        else
        {
            feedbackText.text = "Player with this name already exists.";
        }
    }

    public void UpdateAPIkey()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string playerName = players[currentPlayerIndex];
            string newApiKey = UpdatedapiKeyInput.text;

            if (string.IsNullOrEmpty(newApiKey))
            {
                feedbackText.text = "API key cannot be empty.";
                StartCoroutine(ClearFeedbackText());
                return;
            }

            Player player = DataManager.LoadPlayerData(playerName);
            if (player != null)
            {
                player.ApiKey = newApiKey;
                DataManager.SavePlayerData(player);
                feedbackText.text = "API key updated successfully.";
            }
            else
            {
                feedbackText.text = "Failed to load player data.";
            }
            UpdatedapiKeyInput.text = "";
        }
        else
        {
            feedbackText.text = "No player selected.";
        }
        StartCoroutine(ClearFeedbackText());
    }

    public void DeletePlayer()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string playerName = players[currentPlayerIndex];
            bool playerDeleted = playerManager.DeletePlayer(playerName);

            if (playerDeleted)
            {
                DataManager.DeletePlayerFolder(playerName);
                DataManager.SavePlayerManager(playerManager);

                feedbackText.text = "Player deleted successfully.";
                players = playerManager.PlayerNames;
                currentPlayerIndex = players.Count > 0 ? 0 : -1;
                DisplayCurrentPlayer();
            }
            else
            {
                feedbackText.text = "Player not found.";
            }
        }
        else
        {
            feedbackText.text = "No player selected.";
        }
    }

    public void ShowNewPlayerMenu()
    {
        newPlayerMenu.SetActive(true);
    }

    public void HideNewPlayerMenu()
    {
        newPlayerMenu.SetActive(false);
    }

    public void NavigateLeft()
    {
        if (players.Count > 0)
        {
            currentPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count;
            DisplayCurrentPlayer();
        }
    }

    public void NavigateRight()
    {
        if (players.Count > 0)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            DisplayCurrentPlayer();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowPreviousAdventures()
    {
        Debug.Log("ShowPreviousAdventures called");


        // Clear existing buttons
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            string selectedPlayer = players[currentPlayerIndex];
            Player player = DataManager.LoadPlayerData(selectedPlayer);

            if (player != null)
            {
                foreach (string bookName in player.BookNames)
                {
                    Debug.Log($"Creating button for book: {bookName}");
                    GameObject button = Instantiate(bookButtonPrefab, scrollViewContent.transform);
                    button.SetActive(true);
                    TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = bookName;
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component not found on button prefab.");
                    }

                    Button btnComponent = button.GetComponent<Button>();
                    if (btnComponent != null)
                    {
                        btnComponent.onClick.AddListener(() => OnBookButtonClicked(selectedPlayer, bookName, btnComponent, buttonText));
                    }
                    else
                    {
                        Debug.LogError("Button component not found on button prefab.");
                    }
                }
            }
            else
            {
                feedbackText.text = "Failed to load player data.";
            }
        }
        else
        {
            feedbackText.text = "Please select a player first.";
        }

        previousAdventuresWindow.SetActive(true);
    }

    private void OnBookButtonClicked(string playerName, string bookName, Button button, TMP_Text buttonText)
    {
        Debug.Log($"Button clicked for book: {bookName}");


        // Reset the color of the previously selected button's text
        if (selectedBookButtonText != null)
        {
            selectedBookButtonText.color = new Color(192 / 255f, 0f, 137 / 255f); // Default text color
        }

        // Set the color of the newly selected button's text
        buttonText.color = new Color(123 / 255f, 40 / 255f, 7 / 255f); ; // Selected text color

        // Update the selected button text reference
        selectedBookButtonText = buttonText;


        PlayerSession.SelectedPlayerName = playerName;
        PlayerSession.SelectedBookName = bookName;
        StartPrevAdventure.interactable = true;
        DeletePrevAdventure.interactable = true;

        // Pass the playerName and bookName to the DeletePrevAdventureButtonClicked method
        DeletePrevAdventure.onClick.RemoveAllListeners(); // Clear any previous listeners
        DeletePrevAdventure.onClick.AddListener(() => DeletePrevAdventureButtonClicked(playerName, bookName));
    }

    public void StartPrevAdventureButtonClicked()
    {
        StartPrevAdventure.interactable = false;
        DeletePrevAdventure.interactable = false;
        SceneManager.LoadScene("ViewPrevAdv");
    }

    private void DeletePrevAdventureButtonClicked(string playerName, string bookName)
    {
        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(bookName))
        {
            Player player = DataManager.LoadPlayerData(playerName);
            if (player != null && player.BookNames.Contains(bookName))
            {
                // Remove the book from the player's list
                player.RemoveBook(bookName);
                // Save the updated player data
                DataManager.SavePlayerData(player);

                // Delete the book data from the filesystem
                DataManager.DeleteBookData(playerName, bookName);

                feedbackText.text = $"Book '{bookName}' deleted successfully.";

                // Optionally, update the previous adventures window
                ShowPreviousAdventures();
            }
            else
            {
                feedbackText.text = "Book not found in player data.";
            }
        }
        else
        {
            feedbackText.text = "No book selected to delete.";
        }

        StartPrevAdventure.interactable = false;
        DeletePrevAdventure.interactable = false;

        //StartCoroutine(ClearFeedbackText());
    }



    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackText.text = "";
    }
}
