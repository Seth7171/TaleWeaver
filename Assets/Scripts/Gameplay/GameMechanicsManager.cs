// Filename: GameMechanicsManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages the game mechanics in the interactive storytelling system. Handles various mechanics like combat, luck, riddles, rolls, and checks, and interacts with other components such as the dice roller and player stats.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace TaleWeaver.Gameplay
{
    /// <summary>
    /// Manages different game mechanics (e.g., combat, luck, riddles, rolls, and checks) in the TaleWeaver game.
    /// It controls the flow of mechanics and player interactions, updating player status and sending narrative data to the OpenAI interface.
    /// </summary>
    public class GameMechanicsManager : MonoBehaviour
    {
        public static GameMechanicsManager Instance { get; private set; }
        public string currentMechnism;
        public CreateButtonsInBook optionsMechanics;
        public WordGenerator wordGenerator;

        private System.Random random = new System.Random();

        //lists initialize for mechanics set!
        private List<string> mechanics = new List<string> { "options", "options", "combat", "combat", "luck", "roll", "check" };
        private List<string> pushSenario1 = new List<string> { "+1 life", "-1 life", "+1 luck", "-1 luck", "+1 skillCheck", "-1 skillCheck", "next Is Combat" };
        private List<string> pushSenario2 = new List<string> { "+2 life", "-2 life", "+3 life", "-3 life", "-1 skillCheck", "-1 luck", "-2 luck", "next Is Combat And -1 life" };
        public List<string> rollResults = new List<string> { "-2 life", "-1 life", "Nothing", "+1 luck", "+1 life", "+1 item" };
        private List<(int, int)> combatdifficulties = new List<(int, int)> {
            (5, 15),    // 1-4 - COMBAT DIFFICULTY - 2-15 ( extra layer propability )
            (8, 17),    // 5-9 - COMBAT DIFFICULTY - 8-17
            (14, 19)   // 10 - COMBAT DIFFICULTY - 14-19
        };

        private bool _isSenario2 = false;
        private bool riddleAppeared = false;
        private bool _isNextIsCombat = false;

        private int curRoll = 0;


        private void OnDestroy()
        {
            if (Instance == this)
            {
                // This means this instance was the singleton and is now being destroyed
                Debug.Log("GameMechanicsManager instance is being destroyed.");
                Instance = null;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            // Find the instances of all Mechanics in the scene
            optionsMechanics = FindObjectOfType<CreateButtonsInBook>();

            wordGenerator = new WordGenerator();
        }

        /// <summary>
        /// Initializes the buttons and mechanics in the book.
        /// </summary>
        public void buttonsInit()
        {
            optionsMechanics = FindObjectOfType<CreateButtonsInBook>();
        }

        /// <summary>
        /// Sets the current game mechanic and initializes the necessary components for that mechanic.
        /// </summary>
        /// <param name="mechnism">The mechanic type to set (e.g., combat, roll, luck).</param>
        /// <param name="mechnismOptions">The options for the mechanic.</param>
        public void setMechanism(string mechnism, List<Option> mechnismOptions = null)
        {
            currentMechnism = mechnism;

            if (mechnism.Contains("options"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                }
            }

            if (mechnism.Contains("combat"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                    DiceRoller.Instance.OnIsRollEnded += HandlePlayerRoll;
                }
            }

            if (mechnism.Contains("luck"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                }
            }

            if (mechnism.Contains("riddle"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                }
            }

            if (mechnism.Contains("roll"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                    DiceRoller.Instance.OnIsRollEnded += HandlePlayerRoll;
                }
            }

            if (mechnism.Contains("check"))
            {
                if (optionsMechanics == null)
                {
                    Debug.LogError("OptionsMechanics not found in the scene");
                    return;
                }
                else
                {
                    optionsMechanics.initialize(mechnismOptions);
                    DiceRoller.Instance.OnIsRollEnded += HandlePlayerRoll;
                }
            }
        }

        /// <summary>
        /// Returns a randomly selected game mechanic based on the current page number, with an option to force a combat mechanic.
        /// </summary>
        /// <param name="currPage">The current page number.</param>
        /// <param name="isForceCombat">Whether to force the selection of a combat mechanic.</param>
        /// <returns>The selected mechanic and its details.</returns>
        public string GetRandomMechanic(int currPage, bool isForceCombat = false)
        {
            int chosenMechanic;

            if (isForceCombat)
                chosenMechanic = 2;
            else
                chosenMechanic = random.Next(mechanics.Count);

            if (riddleAppeared)
            {

            }

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! REMOVE THIS LINE WHEN WE WANT RANDOM MECHANICS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //chosenMechanic = 5;
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! REMOVE THIS LINE WHEN WE WANT RANDOM MECHANICS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            if (mechanics[chosenMechanic] == "luck")
            {
                int chosenSenrario1 = random.Next(pushSenario1.Count);
                int chosenSenrario2 = random.Next(pushSenario2.Count);

                return (mechanics[chosenMechanic] + " (Senario1," + pushSenario1[chosenSenrario1] + "), (Senario2, " + pushSenario2[chosenSenrario2] + ")");
            }

            if (mechanics[chosenMechanic] == "riddle")
            {
                mechanics.Remove("riddle");
                wordGenerator = new WordGenerator();
                string randomWord = wordGenerator.GetRandomWord();
                return ("riddle answer is " + randomWord);
            }

            if (mechanics[chosenMechanic] == "combat")
            {
                int diffnum;
                if (currPage >= 1 && currPage <= 4)
                {
                    diffnum = UnityEngine.Random.Range(combatdifficulties[0].Item1, combatdifficulties[0].Item2 + 1); // 1-4 - COMBAT DIFFICULTY - 2-15 ( extra layer propability )
                    if (diffnum == 5)
                    {
                        diffnum = UnityEngine.Random.Range(2, 6);
                    }
                }
                else if (currPage >= 5 && currPage <= 8)
                {
                    diffnum = UnityEngine.Random.Range(combatdifficulties[1].Item1, combatdifficulties[1].Item2 + 1); // 5-9 - COMBAT DIFFICULTY - 8-17
                }
                else
                {
                    diffnum = UnityEngine.Random.Range(combatdifficulties[2].Item1, combatdifficulties[2].Item2 + 1); // 10 - COMBAT DIFFICULTY - 14-19
                }

                return (mechanics[chosenMechanic] + $", difficulty {diffnum}.");
            }

            return mechanics[chosenMechanic];
        }


        /// <summary>
        /// Initiates the adventure by sending the narrative and mechanic data to the OpenAI interface.
        /// </summary>
        /// <param name="bookName">The name of the book.</param>
        /// <param name="narrative">The narrative text.</param>
        public void StartAdventure(string bookName, string narrative)
        {
            mechanics.Add("riddle");

            string mechnism = GetRandomMechanic(1);
            currentMechnism = mechnism;

            if (OpenAIInterface.Instance != null)
            {
                OpenAIInterface.Instance.SendNarrativeToAPI(bookName, narrative + ", mechanic is " + mechnism, 1);
            }
            else
            {
                Debug.LogError("OpenAIInterface instance is not initialized.");
            }

        }

        /// <summary>
        /// Processes the player's choice and updates the mechanic based on the choice.
        /// </summary>
        /// <param name="bookName">The name of the book.</param>
        /// <param name="choice">The player's choice.</param>
        /// <param name="mechnismOption">The selected mechanic option.</param>
        /// <param name="callEndCall">Whether to call the end mechanic after the choice is made.</param>
        public void GetNextMechanicBasedOnChoice(string bookName, string choice)
        {

            if (PlayerInGame.Instance.currentHealth <= 0)
            {
                return;
            }

            BookLoader.Instance.DisableAllCanvases(false);
            BookLoader.Instance.isLoading = true;
            BookLoader.Instance.halfLoadingPage();

            string mechnism;
            if (OpenAIInterface.Instance != null)
            {
                int currPage = OpenAIInterface.Instance.current_Page;
                if (_isNextIsCombat)
                {
                    mechnism = GetRandomMechanic(currPage, _isNextIsCombat);
                    _isNextIsCombat = false;
                }
                else if (currPage == 9)
                {
                    bool forceCombat = true;
                    mechnism = GetRandomMechanic(currPage, forceCombat);
                }
                else if (currPage == 10)
                {
                    PlayerInGame.Instance.PlayerVictory();
                    //OpenAIInterface.Instance.SendMessageToExistingBook(bookName, "combat won, generate conclusion");  We are doing this line in PlayerInGame now
                    return;
                }
                else
                    mechnism = GetRandomMechanic(currPage);

                currentMechnism = mechnism;

                OpenAIInterface.Instance.SendMessageToExistingBook(bookName, choice + ", mechanic is " + mechnism);
            }
            else
            {
                Debug.LogError("OpenAIInterface instance is not initialized.");
            }
        }

        public void HandlePlayerChoice(string bookName, string choice, Option mechnismOption, bool callEndCall = true)
        {
            if (choice.Contains("next Is Combat") || choice.Contains("nextIsCombat"))
            {
                _isNextIsCombat = true;
            }

            if (choice.Contains("Push my luck!"))
            {
                //need to reveal some how the scenario 2
                BookLoader.Instance.RevealLuckSenario2();
                _isSenario2 = true;
                //return to not triger senario 1 outcome.
                return;
            }

            if (_isSenario2)
            {
                mechnismOption = BookLoader.Instance.currentpage.EncounterOptions[1];
            }

            if (choice.Contains("Flee!"))
            {
                BookLoader.Instance.SaveChangedData(0, -1);
                PlayerInGame.Instance.LoseLife(3);
                DiceRoller.Instance.rollButton.gameObject.SetActive(false);
                //ADD LOADING!!!
                GetNextMechanicBasedOnChoice(bookName, "Fled Combat");
                //return to not triger next if cases.
                return;
            }

            if (choice.Contains("Combat won!"))
            {
                BookLoader.Instance.SaveChangedData(0, DiceRoller.Instance.result);
                //ADD LOADING!!!
                GetNextMechanicBasedOnChoice(bookName, "Combat Won");
                return;
            }

            if (choice.Contains("Check Passed !"))
            {
                BookLoader.Instance.SaveChangedData(0, DiceRoller.Instance.result);
                //ADD LOADING!!!
                GetNextMechanicBasedOnChoice(bookName, "Check Passed");
                return;
            }
            if (choice.Contains("Check Failed..."))
            {
                BookLoader.Instance.SaveChangedData(0, DiceRoller.Instance.result);
                //ADD LOADING!!!
                GetNextMechanicBasedOnChoice(bookName, "Check Failed");
                return;
            }

            if (curRoll != 0)
            {
                int roll = curRoll;
                curRoll = 0;
                HandlePlayerChoice(bookName, roll.ToString(), BookLoader.Instance.currentpage.EncounterOptions[roll - 1]);
                //GetNextMechanicBasedOnChoice(bookName, roll.ToString());
                return;
            }

            //Handle life
            if (mechnismOption.outcome.Contains("life"))
            {
                if (mechnismOption.outcome.Contains("-") || mechnismOption.outcome.Contains("lose"))
                {
                    int numberOfLife = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.LoseLife(Math.Abs(numberOfLife));
                }
                if (mechnismOption.outcome.Contains("+") || mechnismOption.outcome.Contains("gain"))
                {
                    int numberOfLife = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.GainLife(numberOfLife);
                }
            }

            //Handle luck
            if (mechnismOption.outcome.Contains("luck"))
            {
                if (mechnismOption.outcome.Contains("-") || mechnismOption.outcome.Contains("lose"))
                {
                    int numberOfLuck = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.LoseLuck(Math.Abs(numberOfLuck));
                }
                if (mechnismOption.outcome.Contains("+") || mechnismOption.outcome.Contains("gain"))
                {
                    int numberOfLuck = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.GainLuck(numberOfLuck);
                }
            }

            //Handle Check modifier
            if (mechnismOption.outcome.Contains("skillCheck"))
            {
                if (mechnismOption.outcome.Contains("-") || mechnismOption.outcome.Contains("lose"))
                {
                    int numberOfSkillCheckModifier = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.LoseSkillModifier(Math.Abs(numberOfSkillCheckModifier));
                }
                if (mechnismOption.outcome.Contains("+") || mechnismOption.outcome.Contains("gain"))
                {
                    int numberOfSkillCheckModifier = GetNumberFromString(mechnismOption.outcome);
                    PlayerInGame.Instance.GainSkillModifier(numberOfSkillCheckModifier);
                }
            }

            // Handle next call
            if (choice.Contains("Accept results and move on"))
            {
                //disable just for debug should work good.
                if (_isSenario2)
                {
                    if (BookLoader.Instance.encounterLuckReward2.text.Contains("next Is Combat") || BookLoader.Instance.encounterLuckReward2.text.Contains("nextIsCombat"))
                        _isNextIsCombat = true;
                    _isSenario2 = false;
                    GetNextMechanicBasedOnChoice(bookName, "scenario2");
                }
                else
                {
                    if (BookLoader.Instance.encounterLuckReward1.text.Contains("next Is Combat") || BookLoader.Instance.encounterLuckReward1.text.Contains("nextIsCombat"))
                        _isNextIsCombat = true;
                    BookLoader.Instance.SaveChangedData(0);
                    GetNextMechanicBasedOnChoice(bookName, "scenario1");
                }
                return;
            }

            if (mechnismOption.outcome.Contains("Wrong"))
            {
                PlayerInGame.Instance.LoseLife(1);
            }

            BookLoader.Instance.SaveChangedData((int)(Char.GetNumericValue(choice[0])) - 1);
            GetNextMechanicBasedOnChoice(bookName, choice);

        }

        public void HandlePlayerRoll(bool isRollEnded)
        {
            if (isRollEnded)
            {
                if (currentMechnism.Contains("combat"))
                {
                    bool combatwon = false;
                    if (BookLoader.Instance.encounterCombatDiff != null)
                    {
                        if (DiceRoller.Instance.result == 20)
                        {
                            PlayerInGame.Instance.GainLuck(1);
                            DiceRoller.Instance.resultText.text = "Critical Success!\n Enemy lose 1 life";
                            combatwon = true;
                        }
                        else if (DiceRoller.Instance.result == 1)
                        {
                            PlayerInGame.Instance.LoseLife(2);
                            DiceRoller.Instance.resultText.text = "Critical Failer!\n YOU lose DOUBLE life";
                        }
                        else if (DiceRoller.Instance.result < int.Parse(BookLoader.Instance.encounterCombatDiff.text))
                        {
                            DiceRoller.Instance.resultText.text = "Failed...\n YOU lose 1 life";
                            PlayerInGame.Instance.LoseLife(1);
                        }
                        else
                        {
                            DiceRoller.Instance.resultText.text = "Success!\n Enemy lose 1 life";
                            combatwon = true;
                        }

                        if (combatwon)
                        {
                            BookLoader.Instance.DiceRollerButton.SetActive(false);
                            BookLoader.Instance.isActionMade = true;
                            //add fades
                            BookLoader.Instance.RevealWon();

                        }
                    }
                }
                if (currentMechnism.Contains("check"))
                {
                    bool checkpassed = false;
                    if (BookLoader.Instance.encounterCheckDiff != null)
                    {
                        if ((DiceRoller.Instance.result + PlayerInGame.Instance.currentSkillModifier) <= 1)
                        {
                            PlayerInGame.Instance.GainLuck(1);
                            DiceRoller.Instance.resultText.text = "Critical Success!\n Check Passed!";
                            checkpassed = true;
                        }
                        else if ((DiceRoller.Instance.result + PlayerInGame.Instance.currentSkillModifier) > int.Parse(BookLoader.Instance.checkDiffNum))
                        {
                            DiceRoller.Instance.resultText.text = "Check Failed...\n YOU lose 1 life";
                            PlayerInGame.Instance.LoseLife(1);
                        }
                        else
                        {
                            DiceRoller.Instance.resultText.text = "Check Passed!";
                            checkpassed = true;
                        }

                        BookLoader.Instance.DiceRollerButton.SetActive(false);
                        BookLoader.Instance.isActionMade = true;
                        //add fades
                        BookLoader.Instance.RevealWon(checkpassed);
                    }
                }

                if (currentMechnism.Contains("roll"))
                {
                    BookLoader.Instance.Dice6Button.SetActive(false);
                    BookLoader.Instance.isActionMade = true;
                    //add fades
                    BookLoader.Instance.RevealRoll(DiceRoller.Instance.result);
                    curRoll = DiceRoller.Instance.result;
                }

            }
        }
        public int GetNumberFromString(string input)
        {
            // Define a regular expression to match numbers (both positive and negative)
            Regex regex = new Regex(@"-?\d+");
            Match match = regex.Match(input);

            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                throw new InvalidOperationException("No number found in the input string.");
            }
        }

    }
}