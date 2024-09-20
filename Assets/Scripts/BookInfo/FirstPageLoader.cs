// Filename: FirstPageLoader.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class is responsible for loading the first page of the book in the application. 
// It loads the book name and description from a JSON file and updates the corresponding UI elements.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

/// <summary>
/// Loads the book data from a JSON file and updates the UI with the book name and description.
/// </summary>
public class FirstPageLoader : MonoBehaviour
{
    // UI Text elements for displaying the book's name and description
    public TextMeshProUGUI bookNameText;
    public TextMeshProUGUI bookDescriptionText;

    // Paths for the book folder and the JSON file storing the book's data
    private string bookFolderPath;
    private string bookFilePath;

    /// <summary>
    /// Initializes the book data paths and triggers the loading of the book information on start.
    /// </summary>
    void Start()
    {
        // Set the folder and file paths based on the selected player's session data
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");

        // Load the book information from the JSON file
        LoadBookInfo();
    }

    /// <summary>
    /// Loads the book information from the book data JSON file and sets the UI elements accordingly.
    /// </summary>
    void LoadBookInfo()
    {
        // Check if the book data file exists
        if (File.Exists(bookFilePath))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(bookFilePath);

            // Deserialize the JSON data into a Book object
            Book bookData = JsonUtility.FromJson<Book>(jsonData);

            // Set the book name and description in the UI
            bookNameText.text = bookData.Name;
            bookDescriptionText.text = bookData.Description;
        }
        else
        {
            // Log an error if the book data file is not found
            Debug.LogError("Book data file not found!");
        }
    }
}
