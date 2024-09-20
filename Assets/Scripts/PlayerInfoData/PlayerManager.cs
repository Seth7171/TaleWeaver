// Filename: PlayerManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages a list of player names, allowing for adding and deleting players.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages a list of player names, providing functionality to add and delete players.
/// </summary>
[Serializable]
public class PlayerManager
{
    public List<string> PlayerNames; // List of player names

    /// <summary>
    /// Constructor for creating a new PlayerManager instance.
    /// Initializes the PlayerNames list.
    /// </summary>
    public PlayerManager()
    {
        PlayerNames = new List<string>(); // Initialize the PlayerNames list
    }

    /// <summary>
    /// Adds a new player name to the list if it does not already exist.
    /// </summary>
    /// <param name="playerName">The name of the player to add.</param>
    /// <returns>True if the player was added, false if they already exist.</returns>
    public bool AddPlayer(string playerName)
    {
        if (PlayerNames.Contains(playerName))
        {
            return false; // Player already exists
        }
        PlayerNames.Add(playerName); // Add the new player
        return true;
    }

    /// <summary>
    /// Deletes a player name from the list.
    /// </summary>
    /// <param name="playerName">The name of the player to delete.</param>
    /// <returns>True if the player was found and deleted, false otherwise.</returns>
    public bool DeletePlayer(string playerName)
    {
        return PlayerNames.Remove(playerName); // Remove the player and return the result
    }
}