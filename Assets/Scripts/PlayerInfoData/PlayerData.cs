// Filename: PlayerData.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages a collection of Player instances, allowing for adding and deleting players.

using System.Collections.Generic;
using System;

/// <summary>
/// Represents a collection of players, allowing for the addition and deletion of player data.
/// </summary>
[Serializable]
public class PlayerData
{
    public List<Player> Players; // List of Player instances

    /// <summary>
    /// Constructor for creating a new PlayerData instance.
    /// Initializes the Players list.
    /// </summary>
    public PlayerData()
    {
        Players = new List<Player>(); // Initialize the Players list
    }

    /// <summary>
    /// Adds a new player to the collection if they do not already exist.
    /// </summary>
    /// <param name="player">The Player instance to add.</param>
    /// <returns>True if the player was added, false if they already exist.</returns>
    public bool AddPlayer(Player player)
    {
        if (Players.Exists(p => p.PlayerName == player.PlayerName))
        {
            return false; // Player already exists
        }
        Players.Add(player); // Add the new player
        return true;
    }

    /// <summary>
    /// Deletes a player from the collection by their name.
    /// </summary>
    /// <param name="playerName">The name of the player to delete.</param>
    /// <returns>True if the player was found and deleted, false if not found.</returns>
    public bool DeletePlayer(string playerName)
    {
        Player player = Players.Find(p => p.PlayerName == playerName);
        if (player != null)
        {
            Players.Remove(player); // Remove the player from the list
            return true; // Player deleted successfully
        }
        return false; // Player not found
    }
}
