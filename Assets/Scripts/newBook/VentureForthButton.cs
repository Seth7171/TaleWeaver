using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class VentureForthButton : MonoBehaviour
{
    public Button ventureForthButton;
    public TMP_InputField bookNameInput;
    public TMP_InputField narrativeInput;
    public TMP_Text feedbackText;

    // Current Player that entered the game
    private PlayerData playerData;
    private Player currentPlayer;

    void Awake()
    {
        string playerName = PlayerSession.SelectedPlayerName;
        string apiKey = PlayerSession.SelectedPlayerApiKey;

        playerData = DataManager.LoadData();
        currentPlayer = playerData.Players.Find(player => player.PlayerName == playerName && player.ApiKey == apiKey);
    }

    public void VentureForth()
    {
        string bookName = bookNameInput.text;

        if (string.IsNullOrEmpty(bookName))
        {
            feedbackText.text = "Book name cannot be empty.";
            StartCoroutine(ClearFeedbackText());
            return;
        }

        // Save the book name in the player's books array
        if (currentPlayer != null)
        {
            currentPlayer.Books.Add(new Book(bookName, ""));
            DataManager.SaveData(playerData);

            feedbackText.text = "Book saved successfully.";
            CreateBookJson();
            StartCoroutine(ClearFeedbackText());
        }
        else
        {
            feedbackText.text = "Player not found.";
            StartCoroutine(ClearFeedbackText());
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void CreateBookJson()
    {
        if (bookNameInput == null || narrativeInput == null)
        {
            Debug.LogError("Input fields are not assigned.");
            return;
        }

        string bookName = bookNameInput.text;
        string narrative = narrativeInput.text;

        if (string.IsNullOrEmpty(bookName) || string.IsNullOrEmpty(narrative))
        {
            Debug.LogError("Book name or narrative is empty.");
            return;
        }

        var bookData = new
        {
            page0 = new
            {
                book_name = bookName,
                narrative = narrative
            }
        };

        string json = JsonUtility.ToJson(bookData, true);
        File.WriteAllText($"{Application.persistentDataPath}/{bookName}.json", json);

        // Call the OpenAI Interface
        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.SendNarrativeToAPI(bookName, narrative);
        }
        else
        {
            Debug.LogError("OpenAIInterface instance is not initialized.");
        }
    }

    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(3);
        feedbackText.text = "";
    }


}
