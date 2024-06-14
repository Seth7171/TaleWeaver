using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class VentureForthButton : MonoBehaviour
{
    public Button ventureForthButton;
    public TMP_InputField bookNameInput;
    public TMP_InputField narrativeInput;
    public TMP_Text feedbackText;

    private Player currentPlayer;

    void Awake()
    {
        string playerName = PlayerSession.SelectedPlayerName;
        currentPlayer = DataManager.LoadPlayerData(playerName);
    }

    public void VentureForth()
    {
        string bookName = bookNameInput.text;
        string narrative = narrativeInput.text;

        if (string.IsNullOrEmpty(bookName))
        {
            feedbackText.text = "Book name cannot be empty.";
            StartCoroutine(ClearFeedbackText());
            return;
        }

        if (currentPlayer != null)
        {
            currentPlayer.BookNames.Add(bookName);
            DataManager.SavePlayerData(currentPlayer);

            Book newBook = new Book(bookName, narrative);
            DataManager.SaveBookData(currentPlayer.PlayerName, newBook);

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

/*        var bookData = new
        {
            page0 = new
            {
                book_name = bookName,
                narrative = narrative
            }
        };

        string json = JsonUtility.ToJson(bookData, true);
        string bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, bookName);
        string bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
        File.WriteAllText(bookFilePath, json);*/

        // Call the OpenAI Interface
        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.SendNarrativeToAPI(bookName, narrative, "1");
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