using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;


public class BookLoader : MonoBehaviour
{
    private OptionsMechanics optionsMechanics;

    public TextMeshProUGUI preEncounterDetails;
    public TextMeshProUGUI encounterNum;
    public TextMeshProUGUI encounterName;
    public TextMeshProUGUI encounterDetails;
    public string encounterMechanic;
    public TextMeshProUGUI encounterMechanicInfo_Options;
    public TextMeshProUGUI encounterOptions1;
    public TextMeshProUGUI encounterOptions2;
    public TextMeshProUGUI encounterOptions3;
    public TextMeshProUGUI encounterOptions1_copy;
    public TextMeshProUGUI encounterOptions2_copy;
    public TextMeshProUGUI encounterOptions3_copy;
    public TextMeshProUGUI encounterMechanicInfo_Riddle;
    public TextMeshProUGUI encounterRiddle1;
    public TextMeshProUGUI encounterRiddle2;
    public TextMeshProUGUI encounterRiddle3;
    public TextMeshProUGUI encounterRiddle1_copy;
    public TextMeshProUGUI encounterRiddle2_copy;
    public TextMeshProUGUI encounterRiddle3_copy;
    public TextMeshProUGUI encounterRoll1;
    public TextMeshProUGUI encounterRoll2;
    public TextMeshProUGUI encounterRoll3;
    public TextMeshProUGUI encounterRoll4;
    public TextMeshProUGUI encounterRoll5;
    public TextMeshProUGUI encounterRoll6;
    public TextMeshProUGUI encounterCheck;
    public TextMeshProUGUI encounterCheckDiff;
    public TextMeshProUGUI encounterCombat;
    public TextMeshProUGUI encounterCombatDiff;
    public TextMeshProUGUI encounterLuck1;
    public TextMeshProUGUI encounterLuck2;
    public TextMeshProUGUI encounterLuckReward1;
    public TextMeshProUGUI encounterLuckReward2;
    public Image encounterImage;

    public GameObject optionsCanvas;
    public GameObject riddleCanvas;
    public GameObject rollCanvas;
    public GameObject checkCanvas;
    public GameObject combatCanvas;
    public GameObject luckCanvas;

    public GameObject optionsUICanvas;
    public GameObject riddleUICanvas;

    public GameObject DiceRoller;
    public GameObject DiceRollerButton;

    private Dictionary<string, int> nameToNumberMap;
    private string objectName;
    private int pageNumBasedon_objectName;

    private string bookFolderPath;
    private string bookFilePath;

    private const int PreEncounterDetailsMaxWords = 100;
    private const int EncounterDetailsMaxWords = 400;
    private const int EncounterMechnicInfoMaxWords = 50;
    private const int EncounterOptionMaxWords = 50;

    void Start()
    {
        // Initialize book paths
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        //bookFolderPath = "C:\\Users\\NitMa\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\moshe\\Shrek\\";
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
        encounterMechanic = page.EncounterMechanic;

        // Reset and disable all canvases
        ResetUIElements();
        DisableAllCanvases();

        // Handle different mechanics
        if (page.EncounterMechanic.StartsWith("$$Roll$$"))
        {
            rollCanvas.SetActive(true);
            DisplayRollOptions(page.EncounterOptions);
        }
        else if (page.EncounterMechanic.StartsWith("&&Riddle&&"))
        {
            riddleCanvas.SetActive(true);
            riddleUICanvas.SetActive(true);
            DisplayRiddle(page.EncounterOptions, page.EncounterMechanicInfo);
        }
        else if (page.EncounterMechanic.StartsWith("!!options!!"))
        {
            optionsCanvas.SetActive(true);
            optionsUICanvas.SetActive(true);
            DisplayOptions(page.EncounterOptions, page.EncounterMechanicInfo);
        }
        else if (page.EncounterMechanic.StartsWith("%%Check%%"))
        {
            checkCanvas.SetActive(true);
            DisplayCheck(page.EncounterOptions);
        }
        else if (page.EncounterMechanic.StartsWith("##Combat##"))
        {
            combatCanvas.SetActive(true);
            DisplayCombat(page.EncounterOptions);
        }
        else if (page.EncounterMechanic.StartsWith("@@luck@@"))
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
        encounterMechanicInfo_Options.text = "";
        encounterOptions1.text = "";
        encounterOptions2.text = "";
        encounterOptions3.text = "";
        encounterOptions1_copy.text = "";
        encounterOptions2_copy.text = "";
        encounterOptions3_copy.text = "";
        encounterMechanicInfo_Riddle.text = "";
        encounterRiddle1.text = "";
        encounterRiddle2.text = "";
        encounterRiddle3.text = "";
        encounterRiddle1_copy.text = "";
        encounterRiddle2_copy.text = "";
        encounterRiddle3_copy.text = "";
        encounterRoll1.text = "";
        encounterRoll2.text = "";
        encounterRoll3.text = "";
        encounterRoll4.text = "";
        encounterRoll5.text = "";
        encounterRoll6.text = "";
        encounterCheck.text = "";
        encounterCheckDiff.text = "";
        encounterCombat.text = "";
        encounterCombatDiff.text = "";
        encounterLuck1.text = "";
        encounterLuck2.text = "";
        encounterLuckReward1.text = "";
        encounterLuckReward2.text = "";
    }

    void DisableAllCanvases()
    {
        optionsCanvas.SetActive(false);
        optionsUICanvas.SetActive(false);
        riddleUICanvas.SetActive(false);
        rollCanvas.SetActive(false);
        checkCanvas.SetActive(false);
        combatCanvas.SetActive(false);
        luckCanvas.SetActive(false);
        DiceRoller.SetActive(false);
        DiceRollerButton.SetActive(false); 
    }

    void DisplayOptions(List<Option> options, string encounterMechanicInfo)
    {
        encounterMechanicInfo_Options.text = "" + encounterMechanicInfo;

        // Display riddle or options
        encounterOptions1.text = options.Count > 0 ? TruncateText(options[0].option, EncounterOptionMaxWords) : "";
        encounterOptions2.text = options.Count > 1 ? TruncateText(options[1].option, EncounterOptionMaxWords) : "";
        encounterOptions3.text = options.Count > 2 ? TruncateText(options[2].option, EncounterOptionMaxWords) : "";

        encounterOptions1_copy.text = encounterOptions1.text;
        encounterOptions2_copy.text = encounterOptions2.text;
        encounterOptions3_copy.text = encounterOptions3.text;

    }

    void DisplayRiddle(List<Option> options, string encounterMechanicInfo)
    {
        encounterMechanicInfo_Riddle.text = "" + encounterMechanicInfo;

        // Display riddle or options
        encounterRiddle1.text = options.Count > 0 ? TruncateText("1). " + options[0].option, EncounterOptionMaxWords) : "";
        encounterRiddle2.text = options.Count > 1 ? TruncateText("2). " + options[1].option, EncounterOptionMaxWords) : "";
        encounterRiddle3.text = options.Count > 2 ? TruncateText("3). " + options[2].option, EncounterOptionMaxWords) : "";

        encounterRiddle1_copy.text = encounterRiddle1.text;
        encounterRiddle2_copy.text = encounterRiddle2.text;
        encounterRiddle3_copy.text = encounterRiddle3.text;
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
        DiceRoller.SetActive(true);
        DiceRollerButton.SetActive(true);
        encounterCheck.text = checkOptions.Count > 0 ? TruncateText(checkOptions[0].option, EncounterOptionMaxWords) : "";
        string checknum = checkOptions.Count > 0 ? TruncateText(checkOptions[0].outcome, EncounterOptionMaxWords) : "";
        encounterCheckDiff.text = $"You need to roll the dice below #{checknum} to pass the Skill Check";
    }

    void DisplayCombat(List<Option> combatOptions)
    {
        // Display combat details
        encounterCombat.text = combatOptions.Count > 0 ? TruncateText(combatOptions[0].option, EncounterOptionMaxWords) : "";
        encounterCombatDiff.text = combatOptions.Count > 0 ? TruncateText(combatOptions[0].outcome, EncounterOptionMaxWords) : "";
    }

    void DisplayLuckOptions(List<Option> luckOptions)
    {
        // Display luck options
        encounterLuck1.text = luckOptions[0].option;
        encounterLuckReward1.text = luckOptions[0].outcome;
        encounterLuckReward1.text = encounterLuckReward1.text.Replace("-", "Lose ").Trim();
        encounterLuckReward1.text = encounterLuckReward1.text.Replace("+", "Gain ").Trim();
        encounterLuck2.text = luckOptions[1].option;
        encounterLuckReward2.text = luckOptions[1].outcome;
        encounterLuckReward2.text = encounterLuckReward2.text.Replace("-", "Lose ").Trim();
        encounterLuckReward2.text = encounterLuckReward2.text.Replace("+", "Gain ").Trim();
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
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        string[] words = text.Split(' ');
        if (words.Length > maxWords)
        {
            return string.Join(" ", words, 0, maxWords) + "...";
        }
        return text;
    }
}
