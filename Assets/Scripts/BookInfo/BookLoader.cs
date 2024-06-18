using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class BookLoader : MonoBehaviour
{
    public TextMeshProUGUI preEncounterDetails;
    public TextMeshProUGUI encounterNum;
    public TextMeshProUGUI encounterName;
    public TextMeshProUGUI encounterDetails;
    public TextMeshProUGUI encounterAction;
    public TextMeshProUGUI encounterOptions;
    public Image encounterImage;

    private string bookFolderPath;
    private string bookFilePath;

    private const int PreEncounterDetailsMaxWords = 100;
    private const int EncounterDetailsMaxWords = 400;
    private const int EncounterActionMaxWords = 50;
    private const int EncounterOptionMaxWords = 50;

    void Start()
    {
        /*bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, "dark castle");*/
        //bookFolderPath = Path.Combine("C:\\Users\\NitMa\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\ron\\space travel\\");
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
            DisplayPage(bookData.Pages[0]);
        }
        else
        {
            Debug.LogError("Book data file not found!");
        }
    }

    void DisplayPage(Page page)
    {
        // Set UI elements
        encounterNum.text = page.EncounterNum;
        encounterName.text = page.EncounterName;
        preEncounterDetails.text = TruncateText(page.EncounterIntroduction, PreEncounterDetailsMaxWords);
        encounterDetails.text = TruncateText(page.EncounterDetails, EncounterDetailsMaxWords);
        encounterAction.text = TruncateText(page.EncounterAction, EncounterActionMaxWords);

        // Parse and truncate choices
        string optionsText = "";
        int i = 0;
        foreach (Option option in page.EncounterOptions)
        {
            i++;
            optionsText += TruncateText($"{i}). "+option.option, EncounterOptionMaxWords) + "\n";
        }
        i = 0;
        encounterOptions.text = optionsText.Trim();

        // Load image
        StartCoroutine(LoadImage(page.ImageUrl));
    }

    IEnumerator LoadImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Image path is null or empty.");
            yield break;
        }

        Debug.Log("Loading image from path: " + imagePath);

        if (File.Exists(imagePath))
        {
            byte[] byteArray = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(byteArray);

            if (encounterImage != null)
            {
                encounterImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Encounter image UI component is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Image file not found at path: " + imagePath);
        }

        yield return null;
    }

    string TruncateText(string text, int maxWords)
    {
        string[] words = text.Split(' ');
        if (words.Length > maxWords)
        {
            return string.Join(" ", words, 0, maxWords) + "...";
        }
        return text;
    }
}
