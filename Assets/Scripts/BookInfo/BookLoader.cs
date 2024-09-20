// Filename: BookLoader.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages the loading, displaying, and interaction of book data within the game. It handles different game mechanics such as options, riddles, rolls, checks, combat, and luck scenarios.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using echo17.EndlessBook;
using System.Xml;
using System;
using UnityEngine.InputSystem.LowLevel;
using echo17.EndlessBook.Demo02;
using TaleWeaver.Gameplay;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]


/// <summary>
/// The BookLoader class is responsible for managing the book-related data and interactions within a game. 
/// It loads and handles different encounter types such as options, riddles, combat, and others, providing dynamic interaction based on player choices.
/// This class also deals with UI updates, scene management, and data persistence related to book interactions.
/// </summary>
public class BookLoader : MonoBehaviour
{
    // Variables for different UI elements and game mechanics
    private CreateButtonsInBook optionsMechanics;

    // Text elements for displaying encounter details
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
    public TextMeshProUGUI encounterRollClick;
    public TextMeshProUGUI encounterRollContinue;
    public TextMeshProUGUI encounterCheck;
    public TextMeshProUGUI encounterCheckDiff;
    public string checkDiffNum;
    public TextMeshProUGUI encounterCheckPass;
    public TextMeshProUGUI encounterCheckFailed;
    public TextMeshProUGUI encounterCombat;
    public TextMeshProUGUI encounterCombatDiff;
    public TextMeshProUGUI encounterCombatFlee;
    public TextMeshProUGUI encounterCombatWon;
    public string combatDiffNum;
    public TextMeshProUGUI encounterLuck1;
    public TextMeshProUGUI encounterLuck2;
    public TextMeshProUGUI encounterLuckReward1;
    public TextMeshProUGUI encounterLuckReward2;
    public TextMeshProUGUI encounterLuckOR;
    public TextMeshProUGUI encounterLuckPush;
    public Image encounterImage;

    // Canvas objects for different game mechanics
    public GameObject optionsCanvas;
    public GameObject riddleCanvas;
    public GameObject rollCanvas;
    public GameObject checkCanvas;
    public GameObject combatCanvas;
    public GameObject luckCanvas;

    // Canvas objects for UI interaction
    public GameObject optionsUICanvas;
    public GameObject riddleUICanvas;
    public GameObject combatUICanvas;
    public TextMeshProUGUI combatFleeUI;
    public TextMeshProUGUI combatWonUI;
    public GameObject rollUICanvas;
    public TextMeshProUGUI rollContinueUI;
    public GameObject checkUICanvas;
    public TextMeshProUGUI checkPassedUI;
    public TextMeshProUGUI checkFailedUI;
    public GameObject luckUICanvas;
    public TextMeshProUGUI luckPushUI;

    // Current UI state management
    public GameObject currentUI;
    public GameObject loadingCanvas;

    // Dice related objects and dice mechanics handling
    public GameObject DiceRoller;
    public GameObject DiceRollerPage;
    public GameObject Dice20;
    public GameObject Dice10;
    public GameObject Dice6;
    public GameObject DiceRollerButton;
    public GameObject Dice20Button;
    public GameObject Dice10Button;
    public GameObject Dice6Button;
    private DiceEuler DiceRotPos;

    // Mapping of names to numbers for pages
    private Dictionary<string, int> nameToNumberMap;
    private string objectName;
    private int pageNumBasedon_objectName;

    // File paths for storing and retrieving book data
    private string bookFolderPath;
    private string bookFilePath;
    public string sceneName;

    // Singleton instance for global access
    public static BookLoader Instance { get; internal set; }
    public Book currentbookData;
    public Page currentpage;
    public Demo02 demo02;

    // Constants for text truncation limits
    private const int PreEncounterDetailsMaxWords = 100;
    private const int EncounterDetailsMaxWords = 400;
    private const int EncounterMechnicInfoMaxWords = 50;
    private const int EncounterOptionMaxWords = 50;

    // Flags for loading and action states
    public bool isLoading = false;
    public bool isActionMade = false;

    // Location strings used in teleportation or scene management
    public string location;
    public string location2;
    public string location3;

    /// <summary>
    /// Initializes the BookLoader component when the game object is activated.
    /// It sets up the scene and book data, including paths and page mappings, ensuring the application is ready to manage book-related interactions.
    /// </summary>
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        // NO MORE SINGELTON STATIC INSTANCE WILL BE CHANGED BETWEEN 1 TO 10 PAGES!
        Instance = this;

        CanvasFader.Instance.InitializeCanvas(loadingCanvas);

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! THIS LINE WHEN WE TO REPLAY A SPACIFIC !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //bookFolderPath = "C:\\Users\\ronsh\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\Sam\\Dungeon\\";
        //bookFolderPath = "C:\\Users\\NitMa\\AppData\\LocalLow\\DefaultCompany\\TaleWeaver\\nitsan\\Desert\\";
        // REMEBER TO COMMENT THE LINE " bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName); "
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! THIS LINE WHEN WE TO REPLAY A SPACIFIC  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // Initialize book paths based on user and selected book
        bookFolderPath = Path.Combine(Application.persistentDataPath, PlayerSession.SelectedPlayerName, PlayerSession.SelectedBookName);
        DataManager.CreateDirectoryIfNotExists(bookFolderPath);
        bookFilePath = Path.Combine(bookFolderPath, "bookData.json");

        // Populate the name-to-number map for page references
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

        // Extract page number based on the GameObject's name
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

        DiceRotPos = new DiceEuler();

        if (sceneName == "ViewPrevAdv")
        {
            GameObject obj = GameObject.Find("Demo");
            if (obj != null)
                demo02 = obj.GetComponent<Demo02>();
        }


        LoadBookData();
    }

    /// <summary>
    /// Loads the book data from a JSON file and displays the current page based on the object's name.
    /// If the page contains a "Conclusion" identifier or any exceptions occur, it logs the details.
    /// Additionally, it updates navigation history in a preview adventure scenario.
    /// </summary>
    void LoadBookData()
    {
        // Check if the book data file exists at the specified path.
        if (File.Exists(bookFilePath))
        {
            // Read the entire JSON file content.
            string jsonData = File.ReadAllText(bookFilePath);
            // Deserialize the JSON data into a Book object.
            currentbookData = JsonUtility.FromJson<Book>(jsonData);
            try
            {
                // Check if the current page is the last in the book and if it marks a conclusion.
                if (currentbookData.Pages.Count - 1 == pageNumBasedon_objectName)
                    if (currentbookData.Pages[pageNumBasedon_objectName].EncounterNum.Contains("Conclusion"))
                        return;// Stop further processing if the page is the conclusion.
                // Display the current page based on the parsed object name index.
                DisplayPage(currentbookData.Pages[pageNumBasedon_objectName]);
                // In a preview adventure scene, update the last page number accessed for UI or navigation purposes.
                if (sceneName == "ViewPrevAdv")
                {
                    if (pageNumBasedon_objectName != 10)// Assuming '10' might be a special case or the last page.
                        demo02.lastPageLoadedNum = Math.Max(demo02.lastPageLoadedNum, pageNumBasedon_objectName + 1);
                }
            }
            // Log the error along with the current maximum page number loaded.
            catch (ArgumentOutOfRangeException ex)
            {
                // Log an error if the book data file does not exist.
                Debug.Log(demo02.lastPageLoadedNum);
            }
        }
        else
        {
            Debug.LogError("Book data file not found!");
        }
    }

    /// <summary>
    /// Saves changes to the book data after a player makes a choice within the game.
    /// This method updates the selected option and associated dice result and then writes the changes to the JSON file.
    /// </summary>
    /// <param name="optionIndx">Index of the option within the current page's options list.</param>
    /// <param name="diceResult">Result of the dice roll associated with the option, defaulting to zero if not specified.</param>
    public void SaveChangedData(int optionIndx, int diceResult = 0)
    {

        // Retrieve the specific option from the current page based on the provided index.
        var encounterOptions = currentbookData.Pages[pageNumBasedon_objectName].EncounterOptions[optionIndx];
        // Check if the retrieved option is not null to prevent runtime errors.
        if (encounterOptions != null)
        {
            // Set the selected answer to true to indicate that this option was chosen.
            encounterOptions.selectedAnswer = true;
            // Update the dice result for the option with the provided or default value.
            encounterOptions.diceResult = diceResult;
        }

        // Convert the updated book data back to a JSON string with nice formatting (pretty print).
        string updatedJson = JsonUtility.ToJson(currentbookData, true);
        File.WriteAllText(bookFilePath, updatedJson);
        Console.WriteLine("Updated JSON data with played choice");

    }

    /// <summary>
    /// Displays the page details in the UI, handles different gameplay mechanics based on the encounter type,
    /// and manages the UI elements associated with each type of encounter.
    /// </summary>
    /// <param name="page">The page object containing all the details to be displayed.</param>
    void DisplayPage(Page page)
    {
        currentpage = page; // Store the current page object for further use.

        // Update UI elements with the data from the current page.
        encounterNum.text = page.EncounterNum;
        encounterName.text = page.EncounterName;
        // Extract and log location data from the page's image and textual content.
        // This should ideally be refactored into a more appropriate place in the code structure.
        location = LocationExtractor.Instance.ExtractLocation(page.ImageGeneration); // FOR NOW HERE, SHOULD NOT BE HERE
        Debug.Log("Extracted Location: " + location);                                             // FOR NOW HERE, SHOULD NOT BE HERE
        location2 = LocationExtractor.Instance.ExtractLocation(page.EncounterIntroduction); // FOR NOW HERE, SHOULD NOT BE HERE
        Debug.Log("Extracted Location2: " + location2);                                             // FOR NOW HERE, SHOULD NOT BE HERE
        location3 = LocationExtractor.Instance.ExtractLocation(page.EncounterDetails); // FOR NOW HERE, SHOULD NOT BE HERE
        Debug.Log("Extracted Location3: " + location3);                                             // FOR NOW HERE, SHOULD NOT BE HERE

        // Truncate text for introduction and details to fit within UI constraints.
        preEncounterDetails.text = TruncateText(page.EncounterIntroduction, PreEncounterDetailsMaxWords);
        encounterDetails.text = TruncateText(page.EncounterDetails, EncounterDetailsMaxWords);
        encounterMechanic = page.EncounterMechanic;

        // Reset and disable all UI canvases to ensure a clean state unless viewing a preview adventure.
        if (sceneName != "ViewPrevAdv")
        {
            ResetUIElements();
            if (TeleportManager.Instance != null)
            {
                // Teleport player or adjust scene based on extracted locations.
                if (location != "None")
                {
                    TeleportManager.Instance.TeleportTo(location);
                }
                else if (location2 != "None")
                {
                    TeleportManager.Instance.TeleportTo(location2);
                }
                else if (location3 != "None")
                {
                    TeleportManager.Instance.TeleportTo(location3);
                }
            }

        }
        DisableAllCanvases();

        // Handle different game mechanics based on the encounter mechanic string identifiers.
        if (page.EncounterMechanic.StartsWith("$$Roll$$"))
        {
            rollCanvas.SetActive(true);
            DisplayRollOptions(page.EncounterOptions);
            if (GameMechanicsManager.Instance != null)
            {
                if (sceneName != "ViewPrevAdv")
                {
                    rollUICanvas.SetActive(true);
                    currentUI = rollUICanvas;
                    rollContinueUI.gameObject.SetActive(false);
                }
                DiceRollerPage.SetActive(true);
                Dice6.SetActive(true);
                if (sceneName != "ViewPrevAdv")
                {
                    DiceRollerButton.SetActive(true);
                    Dice6Button.SetActive(true);
                    GameMechanicsManager.Instance.buttonsInit();
                    GameMechanicsManager.Instance.setMechanism("roll", page.EncounterOptions);
                }
            }
        }
        else if (page.EncounterMechanic.StartsWith("&&Riddle&&"))
        {
            riddleCanvas.SetActive(true);
            DisplayRiddle(page.EncounterOptions, page.EncounterMechanicInfo);
            if (GameMechanicsManager.Instance != null && sceneName != "ViewPrevAdv")
            {
                riddleUICanvas.SetActive(true);
                currentUI = riddleUICanvas;
                GameMechanicsManager.Instance.buttonsInit();
                GameMechanicsManager.Instance.setMechanism("riddle", page.EncounterOptions);
            }
        }
        else if (page.EncounterMechanic.StartsWith("!!options!!"))
        {
            optionsCanvas.SetActive(true);
            DisplayOptions(page.EncounterOptions, page.EncounterMechanicInfo);
            if (GameMechanicsManager.Instance != null && sceneName != "ViewPrevAdv")
            {
                optionsUICanvas.SetActive(true);
                currentUI = optionsUICanvas;
                GameMechanicsManager.Instance.buttonsInit();
                GameMechanicsManager.Instance.setMechanism("options", page.EncounterOptions);
            }
        }
        else if (page.EncounterMechanic.StartsWith("%%Check%%"))
        {
            checkCanvas.SetActive(true);
            DisplayCheck(page.EncounterOptions);
            if (GameMechanicsManager.Instance != null)
            {
                if (sceneName != "ViewPrevAdv")
                {
                    checkUICanvas.SetActive(true);
                    currentUI = checkUICanvas;
                }
                DiceRoller.SetActive(true);
                Dice10.SetActive(true);
                if (sceneName != "ViewPrevAdv")
                {
                    DiceRollerButton.SetActive(true);
                    Dice10Button.SetActive(true);
                    GameMechanicsManager.Instance.buttonsInit();
                    GameMechanicsManager.Instance.setMechanism("check", page.EncounterOptions);
                }

            }
        }
        else if (page.EncounterMechanic.StartsWith("##Combat##"))
        {
            combatCanvas.SetActive(true);
            DisplayCombat(page.EncounterOptions);
            if (GameMechanicsManager.Instance != null)
            {
                if (sceneName != "ViewPrevAdv")
                {
                    combatUICanvas.SetActive(true);
                    currentUI = combatUICanvas;
                }
                DiceRoller.SetActive(true);
                Dice20.SetActive(true);
                if (sceneName != "ViewPrevAdv")
                {
                    DiceRollerButton.SetActive(true);
                    Dice20Button.SetActive(true);
                    GameMechanicsManager.Instance.buttonsInit();
                    GameMechanicsManager.Instance.setMechanism("combat", page.EncounterOptions);
                    if (OpenAIInterface.Instance.current_Page == 10)
                    {
                        encounterCombatFlee.gameObject.SetActive(false);
                        combatFleeUI.gameObject.SetActive(false);
                    }
                }
            }
        }
        else if (page.EncounterMechanic.StartsWith("@@luck@@"))
        {
            luckCanvas.SetActive(true);
            DisplayLuckOptions(page.EncounterOptions);
            if (GameMechanicsManager.Instance != null && sceneName != "ViewPrevAdv")
            {
                luckUICanvas.SetActive(true);
                currentUI = luckUICanvas;
                luckPushUI.gameObject.SetActive(true);
                GameMechanicsManager.Instance.buttonsInit();
                GameMechanicsManager.Instance.setMechanism("luck", page.EncounterOptions);
            }
        }

        // Load image
        StartCoroutine(LoadImage(page.ImageUrl));

        //update health/luck/modifier on ViewPrevAdv
        if (sceneName == "ViewPrevAdv")
        {
            //PlayerInGame.Instance.currentHealth = page.EncounterStats[0];
            GetComponentInChildren<HealthBar>().SetHealth(page.EncounterStats[0]);
            //PlayerInGame.Instance.currentLuck = page.EncounterStats[1];
            GetComponentInChildren<LuckBar>().SetLuck(page.EncounterStats[1]);
            //PlayerInGame.Instance.currentSkillModifier = page.EncounterStats[2];
            GetComponentInChildren<ModifierNum>().SetCheckModifier(page.EncounterStats[2]);
        }
    }

    /// <summary>
    /// Resets all UI text elements associated with game encounters to their default states (empty strings).
    /// This method is used to clear the current state of the UI to ensure that no stale data is displayed
    /// when a new page is loaded or when the game state is reset.
    /// </summary>
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
        checkDiffNum = "";
        encounterCombat.text = "";
        encounterCombatDiff.text = "";
        combatDiffNum = "";
        encounterLuck1.text = "";
        encounterLuck2.text = "";
        encounterLuckReward1.text = "";
        encounterLuckReward2.text = "";
    }

    /// <summary>
    /// Disables all canvases and UI elements related to game mechanics, ensuring a clean UI state.
    /// This method conditionally hides main and specific UI elements based on the game's current state.
    /// </summary>
    /// <param name="isInit">Indicates whether the call is part of an initialization or reset process.</param>
    public void DisableAllCanvases(bool isInit = true)
    {
        // Conditionally hide main game canvases if this is an initialization or a reset event.
        if (isInit)
        {
            // Hide main canvases
            HideGameObject(optionsCanvas, "optionsCanvas");
            HideGameObject(riddleCanvas, "riddleCanvas");
            HideGameObject(rollCanvas, "rollCanvas");
            HideGameObject(checkCanvas, "checkCanvas");
            HideGameObject(combatCanvas, "combatCanvas");
            HideGameObject(luckCanvas, "luckCanvas");
        }

        // Check if the PageFlipper instance is available to manage page transitions.
        if (PageFlipper.Instance != null)
        {
            Debug.Log("PageFlipper.Instance is available");

            // Hide UI-specific canvases and dice elements
            HideGameObject(optionsUICanvas, "optionsUICanvas");
            HideGameObject(riddleUICanvas, "riddleUICanvas");
            if (combatUICanvas != null)
            {
                combatWonUI.gameObject.SetActive(false);
                combatFleeUI.gameObject.SetActive(true);
            }
            HideGameObject(combatUICanvas, "combatUICanvas");
            HideGameObject(rollUICanvas, "rollUICanvas");
            if (checkUICanvas != null)
            {
                checkFailedUI.gameObject.SetActive(false);
                checkPassedUI.gameObject.SetActive(false);
            }
            HideGameObject(checkUICanvas, "checkUICanvas");
            HideGameObject(luckUICanvas, "luckUICanvas");

            if (isInit)
            {
                // Hide dice
                HideGameObject(Dice20, "Dice20");
                HideGameObject(Dice10, "Dice10");
                HideGameObject(DiceRoller, "DiceRoller");

                HideGameObject(Dice6, "Dice6");
                HideGameObject(DiceRollerPage, "DiceRollerPage");

                // Hide dice buttons
                HideGameObject(Dice20Button, "Dice20Button");
                HideGameObject(Dice10Button, "Dice10Button");
                HideGameObject(Dice6Button, "Dice6Button");
                HideGameObject(DiceRollerButton, "DiceRollerButton");
            }

        }
        else
        {
            Debug.LogWarning("PageFlipper.Instance is null.");
        }
    }

    /// <summary>
    /// Hides a specified GameObject if it exists. If the GameObject is null, a warning is logged.
    /// This method is used to manage the visibility of UI elements dynamically, ensuring that
    /// game objects are not visible when not needed, and providing debug information if an object
    /// is unexpectedly missing.
    /// </summary>
    /// <param name="obj">The GameObject to hide.</param>
    /// <param name="name">The name of the GameObject, used for logging purposes.</param>
    private void HideGameObject(GameObject obj, string name)
    {
        // Check if the GameObject reference is not null.
        if (obj != null)
        {
            // Log that the object is being set to inactive, providing traceability for UI state changes.
            Debug.Log($"{name} is set to inactive.");
            obj.SetActive(false);
        }
        else
        {
            // Set the GameObject to inactive, hiding it from view and preventing interaction.
            Debug.LogWarning($"{name} is not assigned.");
        }
    }

    /// <summary>
    /// Updates the UI to display the available options for a specific game mechanic, such as riddles or choices,
    /// and sets the visual feedback for selected answers.
    /// </summary>
    /// <param name="options">List of options available for the current encounter.</param>
    /// <param name="encounterMechanicInfo">Description or additional information about the current encounter mechanic.</param>
    void DisplayOptions(List<Option> options, string encounterMechanicInfo)
    {
        encounterMechanicInfo_Options.text = "" + encounterMechanicInfo;

        // Display riddle or options
        encounterOptions1.text = options.Count > 0 ? TruncateText(options[0].option, EncounterOptionMaxWords) : "";
        encounterOptions2.text = options.Count > 1 ? TruncateText(options[1].option, EncounterOptionMaxWords) : "";
        encounterOptions3.text = options.Count > 2 ? TruncateText(options[2].option, EncounterOptionMaxWords) : "";
        if (sceneName == "ViewPrevAdv")
        {
            if (options[0].selectedAnswer)
            {
                encounterOptions1.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
            if (options[1].selectedAnswer)
            {
                encounterOptions2.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
            if (options[2].selectedAnswer)
            {
                encounterOptions3.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
        }
        if (sceneName != "ViewPrevAdv")
        {
            encounterOptions1_copy.text = encounterOptions1.text;
            encounterOptions2_copy.text = encounterOptions2.text;
            encounterOptions3_copy.text = encounterOptions3.text;
        }

    }

    /// <summary>
    /// Updates the UI to display riddle options and their descriptions for the current encounter.
    /// This method also manages the visual feedback for selected answers and maintains the display consistency across different scenes.
    /// </summary>
    /// <param name="options">List of riddle options available for the current encounter.</param>
    /// <param name="encounterMechanicInfo">Additional descriptive text about the riddle mechanic.</param>
    void DisplayRiddle(List<Option> options, string encounterMechanicInfo)
    {
        encounterMechanicInfo_Riddle.text = "" + encounterMechanicInfo;

        // Display riddle or options
        encounterRiddle1.text = options.Count > 0 ? TruncateText("1). " + options[0].option, EncounterOptionMaxWords) : "";
        encounterRiddle2.text = options.Count > 1 ? TruncateText("2). " + options[1].option, EncounterOptionMaxWords) : "";
        encounterRiddle3.text = options.Count > 2 ? TruncateText("3). " + options[2].option, EncounterOptionMaxWords) : "";
        if (sceneName == "ViewPrevAdv")
        {
            if (options[0].selectedAnswer)
            {
                encounterRiddle1.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
            if (options[1].selectedAnswer)
            {
                encounterRiddle2.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
            if (options[2].selectedAnswer)
            {
                encounterRiddle3.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            }
        }
        if (sceneName != "ViewPrevAdv")
        {
            encounterRiddle1_copy.text = encounterRiddle1.text;
            encounterRiddle2_copy.text = encounterRiddle2.text;
            encounterRiddle3_copy.text = encounterRiddle3.text;
        }
    }

    /// <summary>
    /// Updates the UI to display roll options for a game encounter, showing different outcomes based on the options provided.
    /// It also handles the visual feedback for selected answers in a preview scene.
    /// </summary>
    /// <param name="rollOptions">List of roll options available for the current encounter.</param>
    void DisplayRollOptions(List<Option> rollOptions)
    {
        // Display roll options with outcomes
        encounterRoll1.color = Color.black;
        encounterRoll1.text = rollOptions.Count > 0 ? TruncateText("1). " + rollOptions[0].option, EncounterOptionMaxWords) : "";

        encounterRoll2.color = Color.black;
        encounterRoll2.text = rollOptions.Count > 1 ? TruncateText("2). " + rollOptions[1].option, EncounterOptionMaxWords) : "";

        encounterRoll3.color = Color.black;
        encounterRoll3.text = rollOptions.Count > 2 ? TruncateText("3). " + rollOptions[2].option, EncounterOptionMaxWords) : "";

        encounterRoll4.color = Color.black;
        encounterRoll4.text = rollOptions.Count > 3 ? TruncateText("4). " + rollOptions[3].option, EncounterOptionMaxWords) : "";

        encounterRoll5.color = Color.black;
        encounterRoll5.text = rollOptions.Count > 4 ? TruncateText("5). " + rollOptions[4].option, EncounterOptionMaxWords) : "";

        encounterRoll6.color = Color.black;
        encounterRoll6.text = rollOptions.Count > 5 ? TruncateText("6). " + rollOptions[5].option, EncounterOptionMaxWords) : "";

        if (sceneName == "ViewPrevAdv")
        {
            int curRoll = 0;
            if (rollOptions[0].selectedAnswer)
            {
                encounterRoll1.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 1;
            }
            if (rollOptions[1].selectedAnswer)
            {
                encounterRoll2.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 2;
            }
            if (rollOptions[2].selectedAnswer)
            {
                encounterRoll3.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 3;
            }
            if (rollOptions[3].selectedAnswer)
            {
                encounterRoll4.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 4;
            }
            if (rollOptions[4].selectedAnswer)
            {
                encounterRoll5.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 5;
            }
            if (rollOptions[5].selectedAnswer)
            {
                encounterRoll6.color = new Color(0.5f, 0.0f, 0.0f); // Bordo color
                curRoll = 6;
            }
            if (DiceRotPos.faceRotationsD6.TryGetValue(curRoll, out Quaternion rotation))
            {
                Dice6.transform.rotation = rotation;
            }
            if (DiceRotPos.facePositionsD6.TryGetValue(curRoll, out Vector3 position))
            {
                Dice6.transform.position += position;
            }
        }
    }

    /// <summary>
    /// Displays the conditions and outcomes for a skill check mechanic in the game, adjusting UI elements based on the check's difficulty and result.
    /// </summary>
    /// <param name="checkOptions">List of options describing the check conditions and outcomes.</param>
    void DisplayCheck(List<Option> checkOptions)
    {
        // Display check
        encounterCheck.text = checkOptions.Count > 0 ? TruncateText(checkOptions[0].option, EncounterOptionMaxWords) : "";
        checkDiffNum = checkOptions.Count > 0 ? TruncateText(checkOptions[0].outcome, EncounterOptionMaxWords) : "";
        encounterCheckDiff.text = $"You need to roll the dice below #{checkDiffNum} to pass the Skill Check";
        if (sceneName == "ViewPrevAdv")
        {
            if (checkOptions[0].selectedAnswer)
            {
                encounterCheckPass.gameObject.SetActive(true);
                encounterCheckPass.text = "Check Passed!";
                if (Int32.Parse(checkOptions[0].outcome) <= checkOptions[0].diceResult)
                {
                    encounterCheckPass.text = "Check Failed!";
                }
            }

            if (DiceRotPos.faceRotationsD10.TryGetValue(checkOptions[0].diceResult, out Quaternion rotation))
            {
                Dice10.transform.rotation = rotation;
            }
        }
    }

    /// <summary>
    /// Updates the UI to show combat options and their outcomes, providing visual feedback based on the results of the combat and the player's choices.
    /// </summary>
    /// <param name="combatOptions">List of options related to combat encounters, detailing each possible combat move and its consequences.</param>
    void DisplayCombat(List<Option> combatOptions)
    {
        // Display combat details
        encounterCombat.text = combatOptions.Count > 0 ? TruncateText(combatOptions[0].option, EncounterOptionMaxWords) : "";
        encounterCombatDiff.text = combatOptions.Count > 0 ? TruncateText(combatOptions[0].outcome, EncounterOptionMaxWords) : "";
        combatDiffNum = encounterCombatDiff.text;
        if (sceneName == "ViewPrevAdv")
        {
            if (combatOptions[0].selectedAnswer)
            {

                encounterCombatWon.gameObject.SetActive(true);
                encounterCombatWon.text = "Combat Won!";
                if (combatOptions[0].diceResult == -1)
                {
                    encounterCombatWon.text = "You Fled the Battle!";
                }
            }
            else
            {
                encounterCombatWon.gameObject.SetActive(true);
                encounterCombatWon.text = "You Died!";
            }
            if (DiceRotPos.faceRotationsD20.TryGetValue(combatOptions[0].diceResult, out Quaternion rotation))
            {
                Dice20.transform.rotation = rotation;
            }
        }
    }

    /// <summary>
    /// Displays options for luck-based mechanics, allowing players to see potential rewards or penalties and make informed decisions based on game mechanics.
    /// </summary>
    /// <param name="luckOptions">List of luck-related options that the player can choose from, each with associated outcomes that may affect their game progress.</param>
    void DisplayLuckOptions(List<Option> luckOptions)
    {
        // Display luck options
        encounterLuck1.text = luckOptions[0].option;
        encounterLuckReward1.text = luckOptions[0].outcome;
        if (encounterLuckReward1.text.Contains("-"))
        {
            if (encounterLuckReward1.text.Contains("skillCheck"))
            {
                encounterLuckReward1.color = new Color(0.0f, 0.5f, 0.0f);
            }
            else
            {
                encounterLuckReward1.text = encounterLuckReward1.text.Replace("-", "Lose ").Trim();
                encounterLuckReward1.text = encounterLuckReward1.text.Replace("+", "Gain ").Trim();
                encounterLuckReward1.color = Color.red;
            }
        }
        if (encounterLuckReward1.text.Contains("+"))
        {
            if (encounterLuckReward1.text.Contains("skillCheck"))
            {
                encounterLuckReward1.color = Color.red;
            }
            else
            {
                encounterLuckReward1.text = encounterLuckReward1.text.Replace("-", "Lose ").Trim();
                encounterLuckReward1.text = encounterLuckReward1.text.Replace("+", "Gain ").Trim();
                encounterLuckReward1.color = new Color(0.0f, 0.5f, 0.0f);
            }
        }
        encounterLuck2.text = luckOptions[1].option;
        encounterLuckReward2.text = luckOptions[1].outcome;
        if (encounterLuckReward2.text.Contains("-"))
        {
            if (encounterLuckReward2.text.Contains("skillCheck"))
            {
                encounterLuckReward2.color = new Color(0.0f, 0.5f, 0.0f);
            }
            else
            {
                encounterLuckReward2.text = encounterLuckReward2.text.Replace("-", "Lose ").Trim();
                encounterLuckReward2.text = encounterLuckReward2.text.Replace("+", "Gain ").Trim();
                encounterLuckReward2.color = Color.red;
            }
        }
        if (encounterLuckReward2.text.Contains("+"))
        {
            if (encounterLuckReward2.text.Contains("skillCheck"))
            {
                encounterLuckReward2.color = Color.red;
            }
            else
            {
                encounterLuckReward2.text = encounterLuckReward2.text.Replace("-", "Lose ").Trim();
                encounterLuckReward2.text = encounterLuckReward2.text.Replace("+", "Gain ").Trim();
                encounterLuckReward2.color = new Color(0.0f, 0.5f, 0.0f);
            }
        }
        if (sceneName == "ViewPrevAdv")
        {
            if (luckOptions[0].selectedAnswer)
            {
                encounterLuckPush.text = "You Accepted the Result.";
            }
            else
            {
                encounterLuck1.gameObject.SetActive(false);
                encounterLuckReward1.gameObject.SetActive(false);
                encounterLuck2.gameObject.SetActive(true);
                encounterLuckReward2.gameObject.SetActive(true);
                encounterLuckPush.text = "You Pushed your Luck.";
            }
        }
    }

    /// <summary>
    /// Facilitates the UI transition for a "Luck" scenario, updating visibility and text properties to reflect the outcome of a luck-based decision.
    /// </summary>
    public void RevealLuckSenario2()
    {
        TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { luckPushUI };
        TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { encounterLuck1, encounterLuckReward1, encounterLuckOR, encounterLuckPush };
        TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { encounterLuck2, encounterLuckReward2 };
        TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { encounterLuck2, encounterLuckReward2 };
        TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { encounterLuck1, encounterLuckReward1 };
        ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        SaveChangedData(1);
    }

    /// <summary>
    /// Determines and displays the outcome for combat or check scenarios, dynamically updating UI components based on whether the player passed or failed the challenge.
    /// </summary>
    /// <param name="isPassed">Indicates whether the challenge was passed (true) or failed (false).</param>
    public void RevealWon(bool isPassed = true)
    {
        if (encounterMechanic.Contains("Combat"))
        {
            TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { combatFleeUI };
            TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { encounterCombatFlee };
            TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { combatWonUI, encounterCombatWon };
            TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { encounterCombatWon };
            TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { encounterCombatFlee };
            ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        }
        if (encounterMechanic.Contains("Check"))
        {
            TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { };
            TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { };
            TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { checkPassedUI, encounterCheckPass };
            TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { encounterCheckPass };
            if (!isPassed)
            {
                UICanvasEnable = new TextMeshProUGUI[] { checkFailedUI, encounterCheckFailed };
                textToReveal = new TextMeshProUGUI[] { encounterCheckFailed };
            }
            TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { };
            ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        }

    }

    /// <summary>
    /// Resets the roll options to their default state and hides related UI elements, preparing for another roll action.
    /// </summary>
    public void Reroll6Clicked()
    {
        encounterRoll1.color = Color.black;
        encounterRoll2.color = Color.black;
        encounterRoll3.color = Color.black;
        encounterRoll4.color = Color.black;
        encounterRoll5.color = Color.black;
        encounterRoll6.color = Color.black;
        rollContinueUI.gameObject.SetActive(false);
        encounterRollContinue.gameObject.SetActive(false);

    }

    /// <summary>
    /// Updates the UI to reflect the result of a dice roll, highlighting the roll number that was activated and managing UI visibility.
    /// </summary>
    /// <param name="rollnum">The number result of the dice roll to be displayed prominently.</param>
    public void RevealRoll(int rollnum)
    {
        TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { };
        TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { encounterRollClick };
        TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { rollContinueUI, encounterRollContinue };
        TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { encounterRollContinue };
        TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { encounterRollClick };
        ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);

        TextMeshProUGUI[] textToBordo;
        switch (rollnum)
        {
            case 1:
                textToBordo = new TextMeshProUGUI[] { encounterRoll1 };
                break;
            case 2:
                textToBordo = new TextMeshProUGUI[] { encounterRoll2 };
                break;
            case 3:
                textToBordo = new TextMeshProUGUI[] { encounterRoll3 };
                break;
            case 4:
                textToBordo = new TextMeshProUGUI[] { encounterRoll4 };
                break;
            case 5:
                textToBordo = new TextMeshProUGUI[] { encounterRoll5 };
                break;
            case 6:
                textToBordo = new TextMeshProUGUI[] { encounterRoll6 };
                break;
            default:
                textToBordo = new TextMeshProUGUI[] { };
                break;
        }

        ButtonFader.Instance.FaderBordo(textToBordo);
    }

    /// <summary>
    /// Loads an image from a specified path and applies it to the encounter image UI component. Handles errors if the path is incorrect or the file is missing.
    /// </summary>
    /// <param name="imagePath">The file path of the image to load.</param>
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

    /// <summary>
    /// Truncates a given string to the specified maximum number of words, appending ellipses if the text exceeds this limit.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="maxWords">The maximum number of words allowed before truncation.</param>
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

    /// <summary>
    /// Initiates a fade-in effect for the loading page canvas, gradually revealing it to the user.
    /// </summary>
    public void halfLoadingPage()
    {
        CanvasFader.Instance.FadeInCanvas(loadingCanvas);
    }
}