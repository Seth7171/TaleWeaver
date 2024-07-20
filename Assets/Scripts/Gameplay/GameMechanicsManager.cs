using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMechanicsManager : MonoBehaviour
{
    public static GameMechanicsManager Instance { get; private set; }

    private System.Random random = new System.Random();

    private List<string> mechanics = new List<string> { "options", "combat", "luck", "riddle", "roll", "check" };
    private List<string> pushSenario1 = new List<string> { "+1 life", "-1 life", "+1 luck", "-1 luck", "+1 skillCheck", "-1 skillCheck", "nextIsCombat" };
    private List<string> pushSenario2 = new List<string> { "+2 life", "-2 life", "+3 life", "-3 life", "-1 skillCheck", "-1 luck", "-2 luck", "nextIsCombatAnd-1life" };

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
        if (mechanics[chosenMechanic] == "luck")
        {
            int chosenSenrario1 = random.Next(pushSenario1.Count);
            int chosenSenrario2 = random.Next(pushSenario2.Count);

            return (mechanics[chosenMechanic] + "(Senario1," + pushSenario1[chosenSenrario1] + "), (Senario2, " + pushSenario2[chosenSenrario2] + ")");
        }
        return mechanics[chosenMechanic];
    }

    public string HandlePlayerChoice(string choice, string narrative)
    {
        // Here you can parse the choice and narrative to update player stats
        if (choice.Contains("lose life"))
        {
            //PlayerInGame.Instance.LoseLife(1); // Example of losing life
        }
        // Add more logic based on the narrative

        return GetNextMechanicBasedOnChoice(choice);
    }

    private string GetNextMechanicBasedOnChoice(string choice)
    {
        // Logic to determine the next mechanic based on the player's choice
        if (choice.Contains("nextIsCombat"))
        {
            return "combat";
        }
        // Add more conditions based on the narrative

        return GetRandomMechanic(); // Default to a random mechanic
    }

    public void ParseAPIResponse(string response)
    {
        if (response.Contains("!!options!!"))
        {
            // Extract and handle options
        }

        else if (response.Contains("##Combat##"))
        {
            // Extract and handle combat details
        }

        else if (response.Contains("@@luck@@"))
        {
            // Extract and handle options
        }

        else if (response.Contains("&&Riddle&&"))
        {
            // Extract and handle options
        }

        else if (response.Contains("$$Roll$$"))
        {
            // Extract and handle options
        }

        else if (response.Contains("%%Check%%"))
        {
            // Extract and handle options
        }


        // Example: Reduce player health if the response contains "-1 life"
        if (response.Contains("-1 life"))
        {
            //PlayerInGame.Instance.LoseLife(1);
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
            string encounterAction = "";
            List<Option> choices = new List<Option>();

            // Split the narrative into parts based on the asterisks (**)
            string[] parts = narrative.Split(new[] { "**" }, StringSplitOptions.None);

            if (parts.Length >= 8)
            {
                encounterIntroduction = parts[2].Trim();
                encounterDescription = parts[4].Trim();
                imageGeneration = parts[6].Trim();
                encounterAction = parts[8].Trim();

                // Extract choices or other mechanics
                string[] choiceLines = encounterAction.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                encounterAction = choiceLines[0].Trim();

                if (encounterAction.StartsWith("$$Roll$$"))
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
                else if (encounterAction.StartsWith("&&Riddle&&"))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        var riddleDescription = choiceLines.FirstOrDefault(line => line.StartsWith(i + "."));
                        if (riddleDescription != null)
                        {
                            choices.Add(new Option(riddleDescription.Substring(2).Trim(), "Outcome"));
                        }
                    }
                    // Correct answer
                    var correctAnswerLine = choiceLines.FirstOrDefault(line => line.StartsWith("&&RiddleAns&&"));
                    if (correctAnswerLine != null)
                    {
                        choices.Add(new Option(correctAnswerLine.Replace("&&RiddleAns&&", "").Trim(), "Correct", true));
                    }
                }
                else if (encounterAction.StartsWith("%%Check%%"))
                {
                    string checkNumberLine = choiceLines.FirstOrDefault(line => line.StartsWith("%%"));
                    if (checkNumberLine != null)
                    {
                        string[] checkParts = checkNumberLine.Split(new[] { "%%" }, StringSplitOptions.RemoveEmptyEntries);
                        int checkNumber = int.Parse(checkParts[0].Trim());
                        string checkDescription = checkParts.Length > 1 ? checkParts[1].Trim() : "";
                        choices.Add(new Option($"Check: {checkDescription}", checkNumber.ToString()));
                    }
                }
                else if (encounterAction.StartsWith("##Combat##"))
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
                else if (encounterAction.StartsWith("@@luck@@"))
                {
                    string scenario1 = "";
                    string scenario2 = "";
                    string scenario1Effect = "";
                    string scenario2Effect = "";
                    foreach (var line in choiceLines)
                    {
                        if (line.StartsWith("@@senario 1:@@"))
                        {
                            string[] miniparts = line.Replace("@@senario 1:@@", "").Trim().Split(new[] { "@@- ", "@@luck1Description@@" }, StringSplitOptions.None);
                            scenario1Effect = miniparts[0].Trim();
                            scenario1 = miniparts.Length > 1 ? miniparts[1].Trim() : "";
                        }
                        else if (line.StartsWith("@@senario 2:@@"))
                        {
                            string[] miniparts = line.Replace("@@senario 2:@@", "").Trim().Split(new[] { "@@- ", "@@luck2Description@@" }, StringSplitOptions.None);
                            scenario2Effect = miniparts[0].Trim();
                            scenario2 = miniparts.Length > 1 ? miniparts[1].Trim() : "";
                        }
                    }
                    choices.Add(new Option($"Scenario 1: {scenario1}", scenario1Effect));
                    choices.Add(new Option($"Scenario 2: {scenario2}", scenario2Effect));
                }
                else
                {
                    foreach (var line in choiceLines)
                    {
                        if (line.StartsWith("1.") || line.StartsWith("2.") || line.StartsWith("3."))
                        {
                            choices.Add(new Option(line.Substring(2).Trim(), "Outcome"));
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

            return new Page(encounterNum, encounterName, encounterIntroduction, imageGeneration, encounterDescription, encounterAction, choices, imagePath);
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
