using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string PlayerName;
    public string ApiKey;
    public string assistantID;
    public List<string> BookNames;

    public Player(string playerName, string apiKey, string assistantID)
    {
        this.PlayerName = playerName;
        this.ApiKey = apiKey;
        this.assistantID = assistantID;
        this.BookNames = new List<string>();
    }

    public void RemoveBook(string bookName)
    {
        BookNames.Remove(bookName);
    }
}
