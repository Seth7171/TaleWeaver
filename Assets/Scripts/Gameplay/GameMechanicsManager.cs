using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameMechanicsManager : MonoBehaviour
{
    public static GameMechanicsManager Instance { get; private set; }
    public string currentMechnism;
    private CreateButtonsInBook optionsMechanics;

    private System.Random random = new System.Random();

    private List<string> mechanics = new List<string> { "options", "combat", "luck", "riddle", "roll", "check" };
    private List<string> pushSenario1 = new List<string> { "+1 life", "-1 life", "+1 luck", "-1 luck", "+1 skillCheck", "-1 skillCheck", "nextIsCombat" };
    private List<string> pushSenario2 = new List<string> { "+2 life", "-2 life", "+3 life", "-3 life", "-1 skillCheck", "-1 luck", "-2 luck", "nextIsCombatAnd-1life" };

    private bool _isSenario2 = false;

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

    public string GetRandomMechanic()
    {
        int chosenMechanic = random.Next(mechanics.Count);
        //int chosenMechanic = 5;
        if (mechanics[chosenMechanic] == "luck")
        {
            int chosenSenrario1 = random.Next(pushSenario1.Count);
            int chosenSenrario2 = random.Next(pushSenario2.Count);

            return (mechanics[chosenMechanic] + "(Senario1," + pushSenario1[chosenSenrario1] + "), (Senario2, " + pushSenario2[chosenSenrario2] + ")");
        }
        return mechanics[chosenMechanic];
    }



    public void StartAdventure(string bookName, string narrative)
    {
        string mechnism = GetRandomMechanic();
        currentMechnism = mechnism;
        //GameManager.Instance.setMechanism(mechnism);

        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.SendNarrativeToAPI(bookName, narrative + ", mechanic is " + mechnism, 1);
        }
        else
        {
            Debug.LogError("OpenAIInterface instance is not initialized.");
        }

    }

    public void GetNextMechanicBasedOnChoice(string bookName, string choice)
    {
        string mechnism = GetRandomMechanic();
        currentMechnism = mechnism;
        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.SendNarrativeToAPI(bookName, choice + ", mechanic is " + mechnism, 1);
        }
        else
        {
            Debug.LogError("OpenAIInterface instance is not initialized.");
        }

    }

    // NOT USED YET
    public void HandlePlayerChoice(string bookName, string choice, Option mechnismOption)
    {
        if (choice.Contains("Push my luck!"))
        {
            //need to reveal some how the scenario 2
            BookLoader.Instance.RevealLuckSenario2(true);
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
            PlayerInGame.Instance.LoseLife(Math.Abs(3));
            //disable just for debug should work good.
            GetNextMechanicBasedOnChoice(bookName, "Flee! lose 3 life");
            //return to not triger next if cases.
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
                PlayerInGame.Instance.GainLuck(Math.Abs(numberOfLuck));
            }
            if (mechnismOption.outcome.Contains("+") || mechnismOption.outcome.Contains("gain"))
            {
                int numberOfLuck = GetNumberFromString(mechnismOption.outcome);
                PlayerInGame.Instance.GainLife(numberOfLuck);
            }
        }

        // Handle next call
        if (choice.Contains("Accept results and move on"))
        {
            //disable just for debug should work good.
            if(_isSenario2)
            {
                _isSenario2 = false;
                GetNextMechanicBasedOnChoice(bookName, "scenario2");
            }
            else
            {
                GetNextMechanicBasedOnChoice(bookName, "scenario1");
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

    public Page ParsePage(string narrative, string imagePath)
    {
        try
        {
            // Split the narrative text into lines
            string[] lines = narrative.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Extract Encounter Number and Name
            string encounterLine = lines.FirstOrDefault(line => line.StartsWith("###"));
            if (encounterLine == null) throw new Exception("Encounter line not found");
            string[] encounterParts = encounterLine.Replace("###", "").Trim().Split(new[] { ':' }, 2);
            string encounterNum = encounterParts[0].Trim();
            string encounterName = encounterParts.Length > 1 ? encounterParts[1].Trim() : "";

            // Initialize sections
            string encounterIntroduction = "";
            string encounterDescription = "";
            string imageGeneration = "";
            string encounterMechanic = "";
            string encounterMechanicInfo = "";
            List<Option> choices = new List<Option>();

            // Split the narrative into parts based on the asterisks (**)
            string[] parts = narrative.Split(new[] { "**" }, StringSplitOptions.None);

            if (parts.Length >= 8)
            {
                encounterIntroduction = parts[2].Trim();
                encounterDescription = parts[4].Trim();
                imageGeneration = parts[6].Trim();
                encounterMechanic = parts[8].Trim();

                // Extract choices or other mechanics
                string[] choiceLines = encounterMechanic.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                encounterMechanic = choiceLines[0].Trim();

                if (encounterMechanic.StartsWith("$$Roll$$"))
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        var rollDescription = choiceLines.FirstOrDefault(line => line.StartsWith(i + "."));
                        if (rollDescription != null)
                        {
                            string[] rollParts = rollDescription.Substring(2).Trim().Split(new[] { "$$" }, StringSplitOptions.None);
                            choices.Add(new Option(rollParts[0].Trim(), rollParts.Length > 1 ? rollParts[1].Trim() : ""));
                        }
                    }
                }
                else if (encounterMechanic.StartsWith("&&Riddle&&"))
                {
                    int RiddleInfoIndex = Array.FindIndex(choiceLines, line => line.StartsWith("&&RiddleDescription&&"));
                    if (RiddleInfoIndex != -1)
                    {
                        encounterMechanicInfo = choiceLines[RiddleInfoIndex + 1].Trim();
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        var riddleDescription = choiceLines.FirstOrDefault(line => line.StartsWith(i + "."));
                        if (riddleDescription != null)
                        {
                            choices.Add(new Option(riddleDescription.Substring(2).Trim(), "Outcome"));
                        }
                    }
                    // Correct answer
                    int correctAnswerIndex = Array.FindIndex(choiceLines, line => line.StartsWith("&&RiddleAns&&"));
                    if (correctAnswerIndex != -1)
                    {
                        var correctAnswer = choiceLines[correctAnswerIndex + 1].Trim();
                        string[] AnswerParts = correctAnswer.Split(new[] { '.' }, 2);
                        int correctindx = int.Parse(AnswerParts[0]);
                        choices[correctindx - 1].isCorrectAnswer = true;
                    }
                }
                else if (encounterMechanic.StartsWith("%%Check%%"))
                {
                    int RiddleInfoIndex = Array.FindIndex(choiceLines, line => line.StartsWith("%%CheckDescription%%"));
                    if (RiddleInfoIndex != -1)
                    {
                        string checkDescription = choiceLines[RiddleInfoIndex + 1].Trim();
                        string checkNumber = choiceLines[RiddleInfoIndex + 2].Trim();
                        checkNumber = checkNumber.Replace("%", "");
                        choices.Add(new Option(checkDescription, checkNumber));
                    }
                }
                else if (encounterMechanic.StartsWith("##Combat##"))
                {
                    string monsterName = "";
                    int difficulty = 0;
                    foreach (var line in choiceLines)
                    {
                        if (line.StartsWith("##Name##"))
                        {
                            monsterName = line.Replace("##Name##", "").Trim();
                        }
                        else if (line.StartsWith("##Diff##"))
                        {
                            difficulty = int.Parse(line.Replace("##Diff##", "").Trim());
                        }
                    }
                    choices.Add(new Option($"Combat with {monsterName}", difficulty.ToString()));
                }
                else if (encounterMechanic.StartsWith("@@luck@@"))
                {
                    string scenario1 = "";
                    string scenario2 = "";
                    string scenario1Effect = "";
                    string scenario2Effect = "";
                    int i = 0;
                    foreach (var line in choiceLines)
                    {
                        if (line.StartsWith("@@senario 1:@@"))
                        {
                            //string[] miniparts = line.Replace("@@senario 1:@@", "").Trim().Split(new[] { "@@", "@@luck1Description@@" }, StringSplitOptions.None);
                            //scenario1Effect = miniparts[0].Trim();
                            scenario1Effect = line.Replace("@@senario 1:@@", "").Trim();
                            scenario1Effect = scenario1Effect.Replace("@@", "").Trim();
                            //scenario1 = miniparts.Length > 1 ? miniparts[1].Trim() : "";
                        }
                        if (line.StartsWith("@@luck1Description@@"))
                        {
                            scenario1 = choiceLines[i + 1];
                        }
                        else if (line.StartsWith("@@senario 2:@@"))
                        {
                            //string[] miniparts = line.Replace("@@senario 2:@@", "").Trim().Split(new[] { "@@", "@@luck2Description@@" }, StringSplitOptions.None);
                            //scenario2Effect = miniparts[0].Trim();
                            scenario2Effect = line.Replace("@@senario 2:@@", "").Trim();
                            scenario2Effect = scenario2Effect.Replace("@@", "").Trim();
                            //scenario2 = miniparts.Length > 1 ? miniparts[1].Trim() : "";
                        }
                        if (line.StartsWith("@@luck2Description@@"))
                        {
                            scenario2 = choiceLines[i + 1];
                        }
                        i++;
                    }
                    choices.Add(new Option(scenario1, scenario1Effect));
                    choices.Add(new Option(scenario2, scenario2Effect));
                }
                else
                {
                    bool firstOptionsLine = true;
                    string[] opANDef;
                    string option = "";
                    string effect = "";
                    foreach (var line in choiceLines)
                    {
                        if (line.StartsWith("1.") || line.StartsWith("2.") || line.StartsWith("3."))
                        {
                            opANDef = line.Split(new[] { "!!" }, StringSplitOptions.None);
                            option = opANDef[0];
                            effect = opANDef[1];
                            choices.Add(new Option(option, effect));
                        }
                        else
                        {
                            if (firstOptionsLine && (!line.StartsWith("!!")))
                            {
                                firstOptionsLine = false;
                                encounterMechanicInfo = line;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Narrative format is incorrect.");
            }

            // Truncate sections to their word limits
            encounterIntroduction = TruncateText(encounterIntroduction, 145);
            encounterDescription = TruncateText(encounterDescription, 600);

            return new Page(encounterNum, encounterName, encounterIntroduction, imageGeneration, encounterDescription, encounterMechanic, choices, imagePath, encounterMechanicInfo);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing page: " + ex.Message);
            return null;
        }

    }


    private string TruncateText(string text, int maxWords)
    {
        string[] words = text.Split(' ');
        if (words.Length > maxWords)
        {
            return string.Join(" ", words, 0, maxWords) + "...";
        }
        return text;
    }
}
