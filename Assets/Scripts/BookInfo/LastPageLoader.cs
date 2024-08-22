using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LastPageLoader : MonoBehaviour
{
    public TextMeshProUGUI encounterDetailsDefault;
    public TextMeshProUGUI encounterDetails;
    public Image encounterImage;

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
            if (bookData.Pages[bookData.Pages.Count - 1].EncounterNum.Contains("Conclusion"))
                DisplayPage(bookData.Pages[bookData.Pages.Count - 1]);
        }
        else
        {
            Debug.LogError("Book data file not found!");
        }
    }

    void DisplayPage(Page page)
    {
        // Set UI elements
        encounterDetailsDefault.text = "";

        encounterDetails.gameObject.SetActive(true);
        encounterDetails.text = page.EncounterIntroduction;

        encounterImage.gameObject.SetActive(true);
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
}
