using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LastPageLoader : MonoBehaviour
{
    public TextMeshProUGUI encounterName;
    public TextMeshProUGUI encounterDetails;

    private string bookFolderPath;
    private string bookFilePath;

    // Start is called before the first frame update
    void Start()
    {
        /*bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, "dark castle");*/
        //bookFolderPath = Path.Combine("C:\\Users\\ronsh\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\ron\\dark castle\\");
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");
        LoadBookData();
    }

    void LoadBookData()
    {
        if (File.Exists(bookFilePath))
        {
            string jsonData = File.ReadAllText(bookFilePath);
            Book bookData = JsonUtility.FromJson<Book>(jsonData);
            DisplayPage(bookData.Pages[10]);
        }
        else
        {
            Debug.LogError("Book data file not found!");
        }
    }

    void DisplayPage(Page page)
    {
        // Set UI elements
        encounterName.text = page.EncounterName;
        encounterDetails.text = page.EncounterDetails;
    }
}
