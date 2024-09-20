// Filename: LastPageLoader.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class is responsible for loading and displaying the last page of the book in the application.
// It sets up the UI elements with the encounter details, image, and stats for health and luck.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for loading the last page of a book and displaying encounter details, health, luck, and images.
/// It reads data from a JSON file and updates the respective UI elements accordingly.
/// </summary>
public class LastPageLoader : MonoBehaviour
{
    // UI elements for encounter details and image
    public TextMeshProUGUI encounterDetailsDefault;
    public TextMeshProUGUI encounterDetails;
    public Image encounterImage;

    // UI elements for health and luck bars
    public GameObject healthbar;
    public GameObject luckbar;

    // Paths for the book folder and the JSON file storing the book's data
    private string bookFolderPath;
    private string bookFilePath;

    /// <summary>
    /// Initializes the book data paths and triggers the loading of the last page of the book on start.
    /// </summary>
    void Start()
    {
        // Set the folder and file paths based on the selected player's session data
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");

        // Load the book data
        LoadBookData();
    }

    /// <summary>
    /// Loads the book data from the JSON file and checks if the last page contains the "Conclusion".
    /// If it does, the last page is displayed.
    /// </summary>
    void LoadBookData()
    {
        // Check if the book data file exists
        if (File.Exists(bookFilePath))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(bookFilePath);

            // Deserialize the JSON data into a Book object
            Book bookData = JsonUtility.FromJson<Book>(jsonData);

            // Display the last page if it contains "Conclusion"
            if (bookData.Pages[bookData.Pages.Count - 1].EncounterNum.Contains("Conclusion"))
            {
                DisplayPage(bookData.Pages[bookData.Pages.Count - 1]);
            }
        }
        else
        {
            // Log an error if the book data file is not found
            Debug.LogError("Book data file not found!");
        }
    }

    /// <summary>
    /// Displays the details of the last page including encounter information, image, and health/luck stats.
    /// </summary>
    /// <param name="page">The page object containing the encounter details to display.</param>
    void DisplayPage(Page page)
    {
        // Set UI elements with encounter details
        encounterDetailsDefault.text = "";
        encounterDetails.gameObject.SetActive(true);
        encounterDetails.text = page.EncounterIntroduction;

        // Load and display the encounter image
        encounterImage.gameObject.SetActive(true);
        StartCoroutine(LoadImage(page.ImageUrl));

        // Update health and luck bars based on the page stats
        healthbar.SetActive(true);
        healthbar.GetComponentInChildren<HealthBar>().SetHealth(page.EncounterStats[0]);

        luckbar.SetActive(true);
        luckbar.GetComponentInChildren<LuckBar>().SetLuck(page.EncounterStats[1]);
    }

    /// <summary>
    /// Loads the image for the encounter from the provided file path and assigns it to the encounterImage UI component.
    /// </summary>
    /// <param name="imagePath">The file path of the image to load.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    IEnumerator LoadImage(string imagePath)
    {
        // Validate the image path
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Image path is null or empty.");
            yield break;
        }

        Debug.Log("Loading image from path: " + imagePath);

        // Check if the image file exists
        if (File.Exists(imagePath))
        {
            // Load the image file into a texture
            byte[] byteArray = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(byteArray);

            // Assign the texture to the encounterImage sprite
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
            // Log an error if the image file is not found
            Debug.LogError("Image file not found at path: " + imagePath);
        }

        yield return null;
    }
}
