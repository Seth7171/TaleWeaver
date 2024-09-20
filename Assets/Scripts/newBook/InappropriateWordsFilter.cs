// Filename: InappropriateWordsFilter.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This static class provides functionality to filter inappropriate words from input strings.

using System.Collections.Generic;

/// <summary>
/// This class filters out inappropriate words from input strings.
/// </summary>
public static class InappropriateWordsFilter
{
    // List of inappropriate words
    public static List<string> InappropriateWords = new List<string>
    {
        // Sorry for my language, :)
        "asshole", "bastard", "bitch", "bollocks", "bullshit",
        "cunt", "dick", "dildo", "fag", "fuck", "motherfucker",
        "nigger","nigga", "piss", "prick", "pussy","ass","tits",
        "shit", "slut", "whore", "cock", "fucker", "shithead", "twat",
        "wanker", "rape", "goddamn", "arse", "crap","dickhead","cocksucker",
        "Jesus fuck", "pigfucker", "mother fucker", "dumbass"

    };

    /// <summary>
    /// Checks if the input string contains any inappropriate words.
    /// </summary>
    /// <param name="input">The string to check for inappropriate words.</param>
    /// <returns>True if the input contains inappropriate words; otherwise, false.</returns>
    public static bool ContainsInappropriateWords(string input)
    {
        foreach (string word in InappropriateWords)
        {
            if (input.ToLower().Contains(word.ToLower())) // Check for inappropriate words in a case-insensitive manner
            {
                return true;  // Inappropriate word found
            }
        }
        return false; // No inappropriate words found
    }
}

