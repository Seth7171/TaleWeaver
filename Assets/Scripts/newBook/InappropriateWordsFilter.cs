using System.Collections.Generic;

public static class InappropriateWordsFilter
{
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

    public static bool ContainsInappropriateWords(string input)
    {
        foreach (string word in InappropriateWords)
        {
            if (input.ToLower().Contains(word.ToLower()))
            {
                return true;
            }
        }
        return false;
    }
}

