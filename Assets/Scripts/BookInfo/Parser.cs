using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Parser : MonoBehaviour
{
    public static Parser Instance { get; private set; }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("Parser instance is being destroyed.");
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
                            choices.Add(new Option(rollParts[0].Trim(), GameMechanicsManager.Instance.rollResults[i-1]));
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
                            choices.Add(new Option(riddleDescription.Substring(2).Trim(), "Wrong"));
                        }
                    }
                    // Correct answer
                    int correctAnswerIndex = Array.FindIndex(choiceLines, line => line.StartsWith("&&RiddleAns&&"));
                    if (correctAnswerIndex != -1)
                    {
                        var correctAnswer = choiceLines[correctAnswerIndex + 1].Trim();
                        string[] AnswerParts = correctAnswer.Split(new[] { '.' }, 2);
                        int correctindx = int.Parse(AnswerParts[0]);
                        choices[correctindx - 1].outcome = "Correct";
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
                        if (line.StartsWith("@@scenario 1:@@"))
                        {
                            //string[] miniparts = line.Replace("@@senario 1:@@", "").Trim().Split(new[] { "@@", "@@luck1Description@@" }, StringSplitOptions.None);
                            //scenario1Effect = miniparts[0].Trim();
                            scenario1Effect = line.Replace("@@scenario 1:@@", "").Trim();
                            scenario1Effect = scenario1Effect.Replace("@@", "").Trim();
                            //scenario1 = miniparts.Length > 1 ? miniparts[1].Trim() : "";
                        }
                        if (line.StartsWith("@@luck1Description@@"))
                        {
                            scenario1 = choiceLines[i + 1];
                        }
                        else if (line.StartsWith("@@scenario 2:@@"))
                        {
                            //string[] miniparts = line.Replace("@@senario 2:@@", "").Trim().Split(new[] { "@@", "@@luck2Description@@" }, StringSplitOptions.None);
                            //scenario2Effect = miniparts[0].Trim();
                            scenario2Effect = line.Replace("@@scenario 2:@@", "").Trim();
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
            List<int> encounterStats = new List<int> { 10, 2, 0 };
            if (PlayerInGame.Instance != null)
            {
                encounterStats = new List<int> { PlayerInGame.Instance.currentHealth, PlayerInGame.Instance.currentLuck, PlayerInGame.Instance.currentSkillModifier };
            }
            return new Page(encounterNum, encounterName, encounterIntroduction, imageGeneration, encounterDescription, encounterMechanic, choices, imagePath, encounterStats, encounterMechanicInfo);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing page: " + ex.Message);
            return null;
        }

    }

    public Page ParseConclusion(string messageContent, string imagePath)
    {
        // Split the conclusion content into lines
        string[] lines = messageContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Extract the encounter name and introduction
        string encounterName = "";
        string encounterIntroduction = "";
        bool introductionStarted = false;

        foreach (string line in lines)
        {
            if (line.StartsWith("### Conclusion:"))
            {
                encounterName = line.Replace("### Conclusion:", "").Trim();
            }
            else
            {
                if (!introductionStarted)
                {
                    introductionStarted = true;
                    encounterIntroduction += line;
                }
                else
                {
                    encounterIntroduction += " " + line;
                }
            }
        }

        encounterIntroduction = encounterIntroduction.Replace("^^conclusion^^", "").Trim();

        return new Page(
            encounterNum: "Conclusion",
            encounterName: encounterName,
            encounterIntroduction: encounterIntroduction,
            imageGeneration: "",
            encounterDetails: messageContent,
            encounterMechanic: "",
            encounterMechanicInfo: "",
            encounterOptions: new List<Option>(),
            imageUrl: imagePath,
            encounterStats: new List<int> { 0, 0, 0 }
        );
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

    public string ExtractImageDescription(string messageContent, bool isconc)
    {
        if (string.IsNullOrEmpty(messageContent))
            return null;

        if (isconc)
        {
            string conclusionStartTag = "^^conclusion^^";
            string imageDescriptionTag = "^^conclusion image description^^";

            int conclusionStartIndex = messageContent.IndexOf(conclusionStartTag, StringComparison.OrdinalIgnoreCase);
            int imageDescriptionStartIndex = messageContent.IndexOf(imageDescriptionTag, StringComparison.OrdinalIgnoreCase);

            if (conclusionStartIndex != -1 && imageDescriptionStartIndex != -1)
            {
                string imageGeneration = messageContent.Substring(imageDescriptionStartIndex + imageDescriptionTag.Length).Trim();
                return imageGeneration;
            }
            else
            {
                Debug.LogWarning("Conclusion image description section not found in message content.");
                return null;
            }
        }
        else
        {
            string[] parts = messageContent.Split(new[] { "**" }, StringSplitOptions.None);
            if (parts.Length >= 6)
            {
                string imageGeneration = parts[6].Trim();

                // Optionally, you could split by a known section starter if you want to cut off at that point
                string[] possibleEndTags = new[] { "Encounter Description:", "Mechanics:" };
                foreach (var endTag in possibleEndTags)
                {
                    int endIndex = imageGeneration.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);
                    if (endIndex != -1)
                    {
                        imageGeneration = imageGeneration.Substring(0, endIndex).Trim();
                        break;
                    }
                }

                return imageGeneration;
            }
            else
            {
                Debug.LogWarning("Image generation section not found in message content.");
                return null;
            }
        }
    }

    private string ParseRunId(string errorResponse)
    {
        // Assuming the runId is within the message, we extract it using a simple parsing logic
        var match = System.Text.RegularExpressions.Regex.Match(errorResponse, @"run_(\w+)");
        return match.Success ? match.Value : null;
    }

}
