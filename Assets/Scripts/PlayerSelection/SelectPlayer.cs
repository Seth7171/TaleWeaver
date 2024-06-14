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
    public GameObject newPlayerMenu;
    public Button startNewAdventureButton;
    public Button viewPreviousAdventuresButton;
    public Button deletePlayerButton;
    public Button leftArrowButton;
    public Button rightArrowButton;
    public float displayDuration = 3f;

    private PlayerManager playerManager;
    private List<string> players;
    private int currentPlayerIndex = -1;

    void Start()
    {
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
        }
        else
        {
            playerNameText.text = "";
            startNewAdventureButton.interactable = false;
            viewPreviousAdventuresButton.interactable = false;
            deletePlayerButton.interactable = false;
            leftArrowButton.interactable = false;
            rightArrowButton.interactable = false;
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

    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackText.text = "";
    }
}
