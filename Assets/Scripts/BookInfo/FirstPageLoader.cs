using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class FirstPageLoader : MonoBehaviour
{
    public TextMeshProUGUI bookNameText;
    public TextMeshProUGUI bookDescriptionText;

    private string bookFolderPath;
    private string bookFilePath;

    void Start()
    {
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");

        LoadBookInfo();
    }

    void LoadBookInfo()
    {
        if (File.Exists(bookFilePath))
        {
            string jsonData = File.ReadAllText(bookFilePath);
            Book bookData = JsonUtility.FromJson<Book>(jsonData);

            // Set the book name and description
            bookNameText.text = bookData.Name;
            bookDescriptionText.text = bookData.Description;
        }
        else
        {
            Debug.LogError("Book data file not found!");
        }
    }
}
