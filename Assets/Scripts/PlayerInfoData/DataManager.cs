// Filename: DataManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Manages saving and loading of player and book data, including handling file paths and JSON serialization.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages the loading and saving of player and book data using JSON files.
/// </summary>
public static class DataManager
{
    private static string playerManagerPath = Path.Combine(Application.persistentDataPath, "playerManager.json");

    /// <summary>
    /// Loads the PlayerManager from a JSON file.
    /// </summary>
    /// <returns>A PlayerManager instance.</returns>
    public static PlayerManager LoadPlayerManager()
    {
        if (File.Exists(playerManagerPath))
        {
            string json = File.ReadAllText(playerManagerPath);
            return JsonUtility.FromJson<PlayerManager>(json);
        }
        return new PlayerManager(); // Return a new instance if no file exists
    }

    /// <summary>
    /// Saves the PlayerManager to a JSON file.
    /// </summary>
    /// <param name="playerManager">The PlayerManager instance to save.</param>
    public static void SavePlayerManager(PlayerManager playerManager)
    {
        string json = JsonUtility.ToJson(playerManager, true);
        File.WriteAllText(playerManagerPath, json);
    }

    /// <summary>
    /// Loads player data from a JSON file.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>A Player instance.</returns>
    public static Player LoadPlayerData(string playerName)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, playerName);
        if (Directory.Exists(playerFolderPath))
        {
            string json = File.ReadAllText(Path.Combine(playerFolderPath, "playerData.json"));
            return JsonUtility.FromJson<Player>(json);
        }
        return new Player(playerName, "", ""); // Return a new player if no file exists
    }

    /// <summary>
    /// Saves player data to a JSON file.
    /// </summary>
    /// <param name="player">The Player instance to save.</param>
    public static void SavePlayerData(Player player)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, player.PlayerName);
        CreateDirectoryIfNotExists(playerFolderPath);
        string json = JsonUtility.ToJson(player, true);
        File.WriteAllText(Path.Combine(playerFolderPath, "playerData.json"), json);
    }

    /// <summary>
    /// Creates a folder for a player.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    public static void CreatePlayerFolder(string playerName)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, playerName);
        CreateDirectoryIfNotExists(playerFolderPath);
    }

    /// <summary>
    /// Deletes the folder for a player.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    public static void DeletePlayerFolder(string playerName)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, playerName);
        if (Directory.Exists(playerFolderPath))
        {
            Directory.Delete(playerFolderPath, true);
            Debug.Log($"Player folder deleted at {playerFolderPath}");
        }
        else
        {
            Debug.LogWarning($"Player folder does not exist at {playerFolderPath}");
        }
    }

    /// <summary>
    /// Saves book data to a JSON file.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="book">The Book instance to save.</param>
    public static void SaveBookData(string playerName, Book book)
    {
        string bookFolderPath = Path.Combine(Application.persistentDataPath, playerName, book.Name);
        CreateDirectoryIfNotExists(bookFolderPath);
        string json = JsonUtility.ToJson(book, true);
        File.WriteAllText(Path.Combine(bookFolderPath, "bookData.json"), json);
    }

    /// <summary>
    /// Deletes the folder for a book.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="bookName">The name of the book.</param>
    public static void DeleteBookData(string playerName, string bookName)
    {
        string bookFolderPath = Path.Combine(Application.persistentDataPath, playerName, bookName);
        if (Directory.Exists(bookFolderPath))
        {
            Directory.Delete(bookFolderPath, true);
            Debug.Log($"Book folder deleted at {bookFolderPath}");
        }
        else
        {
            Debug.LogWarning($"Book folder does not exist at {bookFolderPath}");
        }
    }

    /// <summary>
    /// Creates a directory if it does not already exist.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
