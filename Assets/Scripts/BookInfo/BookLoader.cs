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
    public TextMeshProUGUI encounterOptions1;
    public TextMeshProUGUI encounterOptions2;
    public TextMeshProUGUI encounterOptions3;
    public TextMeshProUGUI encounterOptions1_copy;
    public TextMeshProUGUI encounterOptions2_copy;
    public TextMeshProUGUI encounterOptions3_copy;
    public TextMeshProUGUI encounterRoll1;
    public TextMeshProUGUI encounterRoll2;
    public TextMeshProUGUI encounterRoll3;
    public TextMeshProUGUI encounterRoll4;
    public TextMeshProUGUI encounterRoll5;
    public TextMeshProUGUI encounterRoll6;
    public TextMeshProUGUI encounterCheck;
    public TextMeshProUGUI encounterCombat;
    public TextMeshProUGUI encounterLuck1;
    public TextMeshProUGUI encounterLuck2;
    public Image encounterImage;

    public GameObject optionsCanvas;
    public GameObject rollCanvas;
    public GameObject checkCanvas;
    public GameObject combatCanvas;
    public GameObject luckCanvas;

    private Dictionary<string, int> nameToNumberMap;
    private string objectName;
    private int pageNumBasedon_objectName;

    private string bookFolderPath;
    private string bookFilePath;

    private const int PreEncounterDetailsMaxWords = 100;
    private const int EncounterDetailsMaxWords = 400;
    private const int EncounterActionMaxWords = 50;
    private const int EncounterOptionMaxWords = 50;

    void Start()
    {
        // Initialize book paths
/*        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);*/
        bookFolderPath = "C:\\Users\\NitMa\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\moshe\\Hell\\";
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");

        // Initialize the dictionary
        nameToNumberMap = new Dictionary<string, int>
        {
            { "ADV01", 0 },
            { "ADV02", 1 },
            { "ADV03", 2 },
            { "ADV04", 3 },
            { "ADV05", 4 },
            { "ADV06", 5 },
            { "ADV07", 6 },
            { "ADV08", 7 },
            { "ADV09", 8 },
            { "ADV10", 9 }
        };

        // Get the GameObject's name
        objectName = gameObject.name;

        // Try to extract the numeric part of the name
        if (objectName.Length > 3 && int.TryParse(objectName.Substring(3), out int number))
        {
            pageNumBasedon_objectName = number - 1;
            Debug.Log("The number for " + objectName + " is: " + number);
        }
        else
        {
            Debug.LogError("Name " + objectName + " is not in the expected format.");
        }

        LoadBookData();
    }

    void LoadBookData()
    {
        if (File.Exists(bookFilePath))
        {
            string jsonData = File.ReadAllText(bookFilePath);
            Book bookData = JsonUtility.FromJson<Book>(jsonData);
            DisplayPage(bookData.Pages[pageNumBasedon_objectName]);
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

        // Reset and disable all canvases
        ResetUIElements();
        DisableAllCanvases();

        // Handle different mechanics
        if (page.EncounterAction.StartsWith("$$Roll$$"))
        {
            rollCanvas.SetActive(true);
            DisplayRollOptions(page.EncounterOptions);
        }
        else if (page.EncounterAction.StartsWith("&&Riddle&&") || page.EncounterAction.StartsWith("!!options!!"))
        {
            optionsCanvas.SetActive(true);
            DisplayRiddleOrOptions(page.EncounterOptions);
        }
        else if (page.EncounterAction.StartsWith("%%Check%%"))
        {
            checkCanvas.SetActive(true);
            DisplayCheck(page.EncounterOptions);
        }
        else if (page.EncounterAction.StartsWith("##Combat##"))
        {
            combatCanvas.SetActive(true);
            DisplayCombat(page.EncounterOptions);
        }
        else if (page.EncounterAction.StartsWith("@@luck@@"))
        {
            luckCanvas.SetActive(true);
            DisplayLuckOptions(page.EncounterOptions);
        }

        // Load image
        StartCoroutine(LoadImage(page.ImageUrl));
    }

    void ResetUIElements()
    {
        // Clear all UI elements
        encounterOptions1.text = "";
        encounterOptions2.text = "";
        encounterOptions3.text = "";
        encounterOptions1_copy.text = "";
        encounterOptions2_copy.text = "";
        encounterOptions3_copy.text = "";
        encounterRoll1.text = "";
        encounterRoll2.text = "";
        encounterRoll3.text = "";
        encounterRoll4.text = "";
        encounterRoll5.text = "";
        encounterRoll6.text = "";
        encounterCheck.text = "";
        encounterCombat.text = "";
        encounterLuck1.text = "";
        encounterLuck2.text = "";
    }

    void DisableAllCanvases()
    {
        optionsCanvas.SetActive(false);
        rollCanvas.SetActive(false);
        checkCanvas.SetActive(false);
        combatCanvas.SetActive(false);
        luckCanvas.SetActive(false);
    }

    void DisplayRiddleOrOptions(List<Option> options)
    {
        // Display riddle or options
        encounterOptions1.text = options.Count > 0 ? TruncateText("1). " + options[0].option, EncounterOptionMaxWords) : "";
        encounterOptions2.text = options.Count > 1 ? TruncateText("2). " + options[1].option, EncounterOptionMaxWords) : "";
        encounterOptions3.text = options.Count > 2 ? TruncateText("3). " + options[2].option, EncounterOptionMaxWords) : "";

        encounterOptions1_copy.text = encounterOptions1.text;
        encounterOptions2_copy.text = encounterOptions2.text;
        encounterOptions3_copy.text = encounterOptions3.text;
    }

    void DisplayRollOptions(List<Option> rollOptions)
    {
        // Display roll options with outcomes
        encounterRoll1.text = rollOptions.Count > 0 ? TruncateText("1). " + rollOptions[0].option + " - " + rollOptions[0].outcome, EncounterOptionMaxWords) : "";
        encounterRoll2.text = rollOptions.Count > 1 ? TruncateText("2). " + rollOptions[1].option + " - " + rollOptions[1].outcome, EncounterOptionMaxWords) : "";
        encounterRoll3.text = rollOptions.Count > 2 ? TruncateText("3). " + rollOptions[2].option + " - " + rollOptions[2].outcome, EncounterOptionMaxWords) : "";
        encounterRoll4.text = rollOptions.Count > 3 ? TruncateText("4). " + rollOptions[3].option + " - " + rollOptions[3].outcome, EncounterOptionMaxWords) : "";
        encounterRoll5.text = rollOptions.Count > 4 ? TruncateText("5). " + rollOptions[4].option + " - " + rollOptions[4].outcome, EncounterOptionMaxWords) : "";
        encounterRoll6.text = rollOptions.Count > 5 ? TruncateText("6). " + rollOptions[5].option + " - " + rollOptions[5].outcome, EncounterOptionMaxWords) : "";
    }

    void DisplayCheck(List<Option> checkOptions)
    {
        // Display check
        encounterCheck.text = checkOptions.Count > 0 ? TruncateText(checkOptions[0].option, EncounterOptionMaxWords) : "";
    }

    void DisplayCombat(List<Option> combatOptions)
    {
        // Display combat details
        encounterCombat.text = combatOptions.Count > 0 ? TruncateText(combatOptions[0].option, EncounterOptionMaxWords) : "";
    }

    void DisplayLuckOptions(List<Option> luckOptions)
    {
        // Display luck options
        encounterLuck1.text = luckOptions.Count > 0 ? TruncateText("Scenario 1: " + luckOptions[0].option + " - " + luckOptions[0].outcome, EncounterOptionMaxWords) : "";
        encounterLuck2.text = luckOptions.Count > 1 ? TruncateText("Scenario 2: " + luckOptions[1].option + " - " + luckOptions[1].outcome, EncounterOptionMaxWords) : "";
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
