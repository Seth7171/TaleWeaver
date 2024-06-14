using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataManager
{
    private static string playerManagerPath = Path.Combine(Application.persistentDataPath, "playerManager.json");

    public static PlayerManager LoadPlayerManager()
    {
        if (File.Exists(playerManagerPath))
        {
            string json = File.ReadAllText(playerManagerPath);
            return JsonUtility.FromJson<PlayerManager>(json);
        }
        return new PlayerManager();
    }

    public static void SavePlayerManager(PlayerManager playerManager)
    {
        string json = JsonUtility.ToJson(playerManager, true);
        File.WriteAllText(playerManagerPath, json);
    }

    public static Player LoadPlayerData(string playerName)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, playerName);
        if (Directory.Exists(playerFolderPath))
        {
            string json = File.ReadAllText(Path.Combine(playerFolderPath, "playerData.json"));
            return JsonUtility.FromJson<Player>(json);
        }
        return new Player(playerName, "");
    }

    public static void SavePlayerData(Player player)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, player.PlayerName);
        CreateDirectoryIfNotExists(playerFolderPath);
        string json = JsonUtility.ToJson(player, true);
        File.WriteAllText(Path.Combine(playerFolderPath, "playerData.json"), json);
    }

    public static void CreatePlayerFolder(string playerName)
    {
        string playerFolderPath = Path.Combine(Application.persistentDataPath, playerName);
        CreateDirectoryIfNotExists(playerFolderPath);
    }

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

    public static void SaveBookData(string playerName, Book book)
    {
        string bookFolderPath = Path.Combine(Application.persistentDataPath, playerName, book.Name);
        CreateDirectoryIfNotExists(bookFolderPath);
        string json = JsonUtility.ToJson(book, true);
        File.WriteAllText(Path.Combine(bookFolderPath, "bookData.json"), json);
    }

    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
