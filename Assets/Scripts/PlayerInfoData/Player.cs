using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string PlayerName;
    public string ApiKey;
    public List<string> BookNames;

    public Player(string playerName, string apiKey)
    {
        PlayerName = playerName;
        ApiKey = apiKey;
        BookNames = new List<string>();
    }
}
