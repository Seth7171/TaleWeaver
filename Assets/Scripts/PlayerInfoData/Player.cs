using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string PlayerName;
    public string ApiKey;
    public List<string> Books;

    public Player(string playerName, string apiKey)
    {
        PlayerName = playerName;
        ApiKey = apiKey;
        Books = new List<string>();
    }
}
