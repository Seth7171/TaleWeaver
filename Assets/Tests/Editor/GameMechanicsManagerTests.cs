using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TaleWeaver.Gameplay;
using Object = UnityEngine.Object;
using System;

public class GameMechanicsManagerTests
{
    // Declare GameObject variables for cleanup
    private GameObject managerObject;
    private GameObject bookLoaderObject;
    private GameObject playerInGameObject;
    private GameObject diceRollerObject;
    private GameObject pageFlipperObject;
    private GameObject canvasFaderObject;
    private GameObject openAIInterfaceObject;

    private GameMechanicsManager manager;
    private BookLoader bookLoader;
    private PlayerInGame playerInGame;
    private DiceRoller diceRoller;

    private GameObject healthBarObject;
    private GameObject luckBarObject;
    private GameObject modifierNumObject;


    [SetUp]
    public void Setup()
    {

        // Initialize GameMechanicsManager
        managerObject = new GameObject("GameMechanicsManager");
        manager = managerObject.AddComponent<GameMechanicsManager>();
        SetPrivateStaticProperty<GameMechanicsManager>("Instance", manager);

        // Initialize BookLoader
        bookLoaderObject = new GameObject("BookLoader");
        bookLoader = bookLoaderObject.AddComponent<BookLoader>();
        SetPrivateStaticProperty<BookLoader>("Instance", bookLoader);

        // Initialize the necessary TextMeshProUGUI elements used in RevealWon with default text
        bookLoader.combatFleeUI = new GameObject("CombatFleeUI").AddComponent<TextMeshProUGUI>();
        bookLoader.combatFleeUI.text = "a";  // Assign default text

        bookLoader.encounterCombatFlee = new GameObject("EncounterCombatFlee").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterCombatFlee.text = "a";  // Assign default text

        bookLoader.combatWonUI = new GameObject("CombatWonUI").AddComponent<TextMeshProUGUI>();
        bookLoader.combatWonUI.text = "a";  // Assign default text

        bookLoader.encounterCombatWon = new GameObject("EncounterCombatWon").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterCombatWon.text = "a";  // Assign default text

        bookLoader.checkPassedUI = new GameObject("CheckPassedUI").AddComponent<TextMeshProUGUI>();
        bookLoader.checkPassedUI.text = "a";  // Assign default text

        bookLoader.encounterCheckPass = new GameObject("EncounterCheckPass").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterCheckPass.text = "a";  // Assign default text

        bookLoader.checkFailedUI = new GameObject("CheckFailedUI").AddComponent<TextMeshProUGUI>();
        bookLoader.checkFailedUI.text = "a";  // Assign default text

        bookLoader.encounterCheckFailed = new GameObject("EncounterCheckFailed").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterCheckFailed.text = "a";  // Assign default text

        // Initialize ButtonFader
        var buttonFaderObject = new GameObject("ButtonFader");
        var buttonFader = buttonFaderObject.AddComponent<ButtonFader>();
        SetPrivateStaticProperty<ButtonFader>("Instance", buttonFader);

        // Mock ButtonFader.Instance.Fader() to avoid UI-related actions during tests
        buttonFader.Fader(
            new TextMeshProUGUI[] { },
            new TextMeshProUGUI[] { },
            new TextMeshProUGUI[] { },
            new TextMeshProUGUI[] { },
            new TextMeshProUGUI[] { });

        // Initialize PlayerInGame
        playerInGameObject = new GameObject("PlayerInGame");
        playerInGame = playerInGameObject.AddComponent<PlayerInGame>();
        SetPrivateStaticProperty<PlayerInGame>("Instance", playerInGame);

        // Initialize DiceRoller
        diceRollerObject = new GameObject("DiceRoller");
        diceRollerObject.AddComponent<BoxCollider>();
        diceRoller = diceRollerObject.AddComponent<DiceRoller>();
        SetPrivateStaticProperty<DiceRoller>("Instance", diceRoller);
        // Assign Button component to rollButton
        diceRoller.rollButton = new GameObject("RollButton").AddComponent<Button>();

        // Initialize encounterCombatDiff in BookLoader
        var combatDiffObject = new GameObject("CombatDiffText");
        bookLoader.encounterCombatDiff = combatDiffObject.AddComponent<TextMeshProUGUI>();

        // Initialize encounterCheckDiff in BookLoader
        var checkDiffObject = new GameObject("CombatDiffText");
        bookLoader.encounterCheckDiff = checkDiffObject.AddComponent<TextMeshProUGUI>();

        // Initialize encounterCheckDiff in BookLoader
        var checkDiffNumObject = "CombatDiffNumText";
        bookLoader.checkDiffNum = checkDiffNumObject;

        // Initialize resultText in DiceRoller
        var resultTextObject = new GameObject("ResultText");
        diceRoller.resultText = resultTextObject.AddComponent<TextMeshProUGUI>();
        diceRoller.resultText.text = "Default Result";  // Set an initial value for testing

        // Initialize PageFlipper
        pageFlipperObject = new GameObject("PageFlipper");
        var pageFlipper = pageFlipperObject.AddComponent<PageFlipper>();
        SetPrivateStaticProperty<PageFlipper>("Instance", pageFlipper);

        // Initialize CanvasFader
        canvasFaderObject = new GameObject("CanvasFader");
        var canvasFader = canvasFaderObject.AddComponent<CanvasFader>();
        CanvasFader.Instance = canvasFader;

        // Initialize OpenAIInterface
        openAIInterfaceObject = new GameObject("OpenAIInterface");
        var openAIInterface = openAIInterfaceObject.AddComponent<OpenAIInterface>();
        OpenAIInterface.Instance = openAIInterface;

        // Initialize PlayerInGame properties
        playerInGame.currentHealth = 10;
        playerInGame.currentLuck = 5;
        playerInGame.currentSkillModifier = 0;
        playerInGame.audioSource = playerInGameObject.AddComponent<AudioSource>();
        playerInGame.decisions_Canvas = new GameObject("DecisionsCanvas");
        playerInGame.DeathScreen = new GameObject("DeathScreen");
        playerInGame.VictoryScreen = new GameObject("VictoryScreen");
        // Initialize the audio clip for TakeDamageChosen to avoid null error
        playerInGame.audioSource = playerInGameObject.AddComponent<AudioSource>();
        playerInGame.TakeDamageChosen = AudioClip.Create("MockClip", 44100, 1, 44100, false); // Mock clip
        playerInGame.gender = new GameObject("GenderText").AddComponent<TextMeshProUGUI>();
        playerInGame.gender.text = "Male";
        playerInGame.handBookController = playerInGameObject.AddComponent<HandBookController>();

        // Continue with other setup, such as initializing other UI elements, the health bar, and luck bar
        // Mock the HealthBar
        healthBarObject = new GameObject("HealthBar");
        var healthBar = healthBarObject.AddComponent<HealthBar>();
        healthBar.slider = healthBarObject.AddComponent<Slider>();
        healthBar.HP_text = healthBarObject.AddComponent<TextMeshProUGUI>();
        SetPrivateStaticProperty<HealthBar>("Instance", healthBar);

        // Mock the LuckBar
        luckBarObject = new GameObject("LuckBar");
        var luckBar = luckBarObject.AddComponent<LuckBar>();
        luckBar.slider = luckBarObject.AddComponent<Slider>();
        luckBar.Luck_text = luckBarObject.AddComponent<TextMeshProUGUI>();
        SetPrivateStaticProperty<LuckBar>("Instance", luckBar);

        // Mock the ModifierNum
        modifierNumObject = new GameObject("ModifierNum");
        var modifierNum = modifierNumObject.AddComponent<ModifierNum>();
        modifierNum.Check_Modifier_text = modifierNumObject.AddComponent<TextMeshProUGUI>();
        SetPrivateStaticProperty<ModifierNum>("Instance", modifierNum);

        // Mock the `redScreen` GameObject in PlayerInGame
        playerInGame.redScreen = new GameObject("RedScreen");
        playerInGame.redScreen.SetActive(false);  // Mock it being off initially

        // Initialize required components and properties in BookLoader
        bookLoader.currentbookData = new Book("TestBook", "Test Description");

        // Initialize test page and currentpage with default options
        var encounterOptions = new List<Option>
        {
            new Option("1) Option 1", "Outcome 1"),
            new Option("2) Option 2", "Outcome 2"),
            new Option("3) Option 3", "Outcome 3")
        };
        var testPage = new Page
        {
            EncounterNum = "1",
            EncounterName = "Test Encounter",
            EncounterIntroduction = "Introduction",
            EncounterDetails = "Details",
            EncounterMechanic = "!!options!!",
            EncounterMechanicInfo = "Mechanic Info",
            EncounterOptions = encounterOptions,
            EncounterStats = new List<int> { 10, 2, 0 }
        };
        bookLoader.currentbookData.Pages = new List<Page> { testPage };
        bookLoader.currentpage = testPage;

        // Initialize UI elements in BookLoader
        bookLoader.encounterLuckReward1 = new GameObject("EncounterLuckReward1").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterLuckReward1.text = "some text";

        bookLoader.encounterLuckReward2 = new GameObject("EncounterLuckReward2").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterLuckReward2.text = "some text";

        bookLoader.encounterLuckPush = new GameObject("EncounterLuckPush").AddComponent<TextMeshProUGUI>();
        bookLoader.encounterLuckPush.text = "Push my luck!";

        bookLoader.encounterMechanicInfo_Options = new GameObject("EncounterMechanicInfo_Options").AddComponent<TextMeshProUGUI>();

        // Initialize other necessary UI elements or components in BookLoader
        // Assign Button component to DiceRollerButton
        bookLoader.DiceRollerButton = new GameObject("DiceRollerButton");

        // Initialize UI canvas variables in BookLoader
        bookLoader.optionsUICanvas = new GameObject("OptionsUICanvas");
        bookLoader.riddleUICanvas = new GameObject("RiddleUICanvas");
        bookLoader.combatUICanvas = new GameObject("CombatUICanvas");
        bookLoader.rollUICanvas = new GameObject("RollUICanvas");
        bookLoader.checkUICanvas = new GameObject("CheckUICanvas");
        bookLoader.luckUICanvas = new GameObject("LuckUICanvas");

        // Initialize combat UI elements in BookLoader as TextMeshProUGUI components
        bookLoader.combatWonUI = new GameObject("CombatWonUI").AddComponent<TextMeshProUGUI>();
        bookLoader.combatFleeUI = new GameObject("CombatFleeUI").AddComponent<TextMeshProUGUI>();

        // Initialize check UI elements in BookLoader as TextMeshProUGUI components
        bookLoader.checkFailedUI = new GameObject("CheckFailedUI").AddComponent<TextMeshProUGUI>();
        bookLoader.checkPassedUI = new GameObject("CheckPassedUI").AddComponent<TextMeshProUGUI>();

        // Initialize dice GameObjects in BookLoader
        bookLoader.Dice20 = new GameObject("Dice20");
        bookLoader.Dice10 = new GameObject("Dice10");
        bookLoader.DiceRoller = new GameObject("DiceRoller");
        bookLoader.Dice6 = new GameObject("Dice6");
        bookLoader.DiceRollerPage = new GameObject("DiceRollerPage");

        // Initialize dice buttons in BookLoader with Button components
        bookLoader.Dice20Button = new GameObject("Dice20Button");
        bookLoader.Dice10Button = new GameObject("Dice10Button");
        bookLoader.Dice6Button = new GameObject("Dice6Button");
        // DiceRollerButton is already initialized above with a Button component

        // Initialize properties in GameMechanicsManager if needed
        manager.currentMechnism = "initialMechanism";

        // Initialize file paths
        var testBookFolderPath = Path.Combine(Application.persistentDataPath, "TestPlayer", "TestBook");
        var testBookFilePath = Path.Combine(testBookFolderPath, "bookData.json");

        // Set the private fields in BookLoader
        SetPrivateField(bookLoader, "bookFolderPath", testBookFolderPath);
        SetPrivateField(bookLoader, "bookFilePath", testBookFilePath);

        // Create the directory and a placeholder file
        Directory.CreateDirectory(testBookFolderPath);
        File.WriteAllText(testBookFilePath, "{}");

        // Set 'pageNumBasedon_objectName' to 0 (or appropriate index)
        SetPrivateField(bookLoader, "pageNumBasedon_objectName", 0);
    }

    [UnityTest]
    public IEnumerator TestPlayerChoiceProcessing()
    {
        // Arrange is already handled in [SetUp]

        // Act
        manager.HandlePlayerChoice("TestBook", "1) Option 1", new Option("1) Option 1", "Outcome 1"), false);

        // Assert
        Assert.IsTrue(manager.currentMechnism.Contains("combat") ||
                      manager.currentMechnism.Contains("options") ||
                      manager.currentMechnism.Contains("roll") ||
                      manager.currentMechnism.Contains("riddle") ||
                      manager.currentMechnism.Contains("check") ||
                      manager.currentMechnism.Contains("luck"),
                      "currentMechanism did not contain any expected values after HandlePlayerChoice.");

        yield return null; // Wait for a frame if necessary
    }

    [UnityTest]
    public IEnumerator TestHandlePlayerChoice_ModifyPlayerStats()
    {
        // Arrange
        var modifyOptions = new List<Option>
        {
            new Option("1) Gain Life", "+2 life"),
            new Option("2) Lose Life", "-3 life"),
            new Option("3) Lose Luck", "lose 1 luck"),
            new Option("4) Gain Luck", "gain 1 luck"),
            new Option("5) Gain Skill Check", "+1 skillCheck"),
            new Option("6) Lose Skill Check", "-1 skillCheck")
        };

        // Update the current page's EncounterOptions with the new options
        bookLoader.currentpage.EncounterOptions = modifyOptions;

        // Act & Assert for each option
        foreach (var option in modifyOptions)
        {
            // Reset PlayerInGame's stats before each choice
            playerInGame.currentHealth = 10;
            playerInGame.currentLuck = 4;
            playerInGame.currentSkillModifier = 0;

            // Act
            manager.HandlePlayerChoice("TestBook", option.option, option, false);

            // Assert based on the option's effect
            switch (option.outcome.ToLower())
            {
                case "+2 life":
                    Assert.AreEqual(12, playerInGame.currentHealth, "Gain Life option did not correctly add 2 life.");
                    break;
                case "-3 life":
                    Assert.AreEqual(7, playerInGame.currentHealth, "Lose Life option did not correctly subtract 3 life.");
                    break;
                case "lose 1 luck":
                    Assert.AreEqual(3, playerInGame.currentLuck, "Lose Luck option did not correctly subtract luck.");
                    break;
                case "gain 1 luck":
                    Assert.AreEqual(5, playerInGame.currentLuck, "Gain Luck option did not correctly add luck.");
                    break;
                case "+1 skillcheck":
                    Assert.AreEqual(1, playerInGame.currentSkillModifier, "Gain Skill Check option did not correctly add skill check.");
                    break;
                case "-1 skillcheck":
                    Assert.AreEqual(-1, playerInGame.currentSkillModifier, "Lose Skill Check option did not correctly subtract skill check.");
                    break;
                default:
                    Assert.Fail($"Unhandled option effect: {option.outcome}");
                    break;
            }
        }

        yield return null; // Wait for a frame if necessary
    }

    [UnityTest]
    public IEnumerator TestHandlePlayerRoll_CombatOutcome()
    {
        // Arrange
        bookLoader.encounterMechanic = "Combat";
        bookLoader.encounterCombatDiff.text = "5"; // Combat difficulty is 5

        // Test Case 1: Combat Failure (dice result < combat difficulty)
        diceRoller.result = 3; // Less than 5

        // Act
        manager.currentMechnism = "combat";
        manager.HandlePlayerRoll(true);

        // Assert
        Assert.AreEqual(9, playerInGame.currentHealth, "Combat failure did not correctly subtract 1 life.");
        Assert.AreEqual("Failed...\n YOU lose 1 life", diceRoller.resultText.text, "Incorrect failure message.");

        // Reset player life
        playerInGame.currentHealth = 10;

        // Test Case 2: Combat Success (dice result >= combat difficulty)
        diceRoller.result = 6; // Equal to combat difficulty

        // Act
        manager.HandlePlayerRoll(true);

        // Assert
        Assert.AreEqual(10, playerInGame.currentHealth, "Combat success incorrectly modified player life.");
        Assert.AreEqual("Success!\n Enemy lose 1 life", diceRoller.resultText.text, "Incorrect success message.");

        yield return null; // Wait for a frame if necessary
    }

    [UnityTest]
    public IEnumerator TestHandlePlayerRoll_SkillCheckOutcome()
    {
        // Arrange
        bookLoader.encounterMechanic = "Check";
        bookLoader.encounterCheckDiff.text = "4"; // Skill check difficulty is 4
        bookLoader.checkDiffNum = "4";
        playerInGame.currentSkillModifier = 2;

        // Test Case 1: Skill Check Failure
        diceRoller.result = 8;

        // Act
        manager.currentMechnism = "skillcheck";
        manager.HandlePlayerRoll(true);

        // Assert
        Assert.AreEqual(9, playerInGame.currentHealth, "Skill check failure did not correctly subtract 1 life.");
        Assert.AreEqual("Check Failed...\n YOU lose 1 life", diceRoller.resultText.text, "Incorrect failure message.");

        // Reset player life
        playerInGame.currentHealth = 10;

        // Test Case 2: Skill Check Success
        diceRoller.result = 1;

        // Act
        manager.HandlePlayerRoll(true);

        // Assert
        Assert.AreEqual(10, playerInGame.currentHealth, "Skill check success incorrectly modified player life.");
        Assert.AreEqual("Check Passed!", diceRoller.resultText.text, "Incorrect success message.");

        yield return null; // Wait for a frame if necessary
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up created GameObjects to prevent interference between tests
        Object.DestroyImmediate(managerObject);
        Object.DestroyImmediate(bookLoaderObject);
        Object.DestroyImmediate(playerInGameObject);
        Object.DestroyImmediate(diceRollerObject);
        Object.DestroyImmediate(pageFlipperObject);
        Object.DestroyImmediate(canvasFaderObject);
        Object.DestroyImmediate(openAIInterfaceObject);
        Object.DestroyImmediate(healthBarObject);
        Object.DestroyImmediate(luckBarObject);
        Object.DestroyImmediate(modifierNumObject);
    }

    // Helper method to set private instance fields
    public static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            throw new Exception($"Field '{fieldName}' not found in object of type '{obj.GetType().FullName}'.");
        }
    }

    // Helper method to set private static properties
    public static void SetPrivateStaticProperty<T>(string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (property != null)
        {
            var setter = property.GetSetMethod(true);
            if (setter != null)
            {
                setter.Invoke(null, new object[] { value });
            }
            else
            {
                throw new Exception($"Property '{propertyName}' does not have a setter.");
            }
        }
        else
        {
            throw new Exception($"Property '{propertyName}' not found in type '{typeof(T).FullName}'.");
        }
    }

    public static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
        else
        {
            throw new Exception($"Property '{propertyName}' not found or cannot be written in object of type '{obj.GetType().FullName}'.");
        }
    }



}
