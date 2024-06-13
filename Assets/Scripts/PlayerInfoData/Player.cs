using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string PlayerName;
    public string ApiKey;
    public List<Book> Books;

    public Player(string playerName, string apiKey)
    {
        PlayerName = playerName;
        ApiKey = apiKey;
        Books = new List<Book>();
    }
}
