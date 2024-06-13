using UnityEngine;
using System.IO;

public static class DataManager
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "playerData.json");

    public static void SaveData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(filePath, json);
    }

    public static PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return new PlayerData();
    }
}