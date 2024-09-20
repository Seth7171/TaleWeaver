// Filename: Player.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Represents a player in the game, including player data such as name, API key, and associated book names.

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a player, holding their name, API key, assistant ID, and a list of book names.
/// </summary>
[Serializable]
public class Player
{
    public string PlayerName; // The name of the player
    public string ApiKey; // The API key for the player
    public string assistantID; // The assistant ID for the player
    public List<string> BookNames; // List of book names associated with the player

    /// <summary>
    /// Constructor for creating a new Player instance.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="apiKey">The API key for the player.</param>
    /// <param name="assistantID">The assistant ID for the player.</param>
    public Player(string playerName, string apiKey, string assistantID)
    {
        this.PlayerName = playerName;
        this.ApiKey = apiKey;
        this.assistantID = assistantID;
        this.BookNames = new List<string>(); // Initialize the BookNames list
    }

    /// <summary>
    /// Removes a book from the player's list of book names.
    /// </summary>
    /// <param name="bookName">The name of the book to remove.</param>
    public void RemoveBook(string bookName)
    {
        BookNames.Remove(bookName); // Remove the specified book from the list
    }
}
