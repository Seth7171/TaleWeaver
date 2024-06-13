using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataManager
{
    private static string dataPath = Application.persistentDataPath + "/playerData.json";

    public static PlayerData LoadData()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return new PlayerData();
    }

    public static void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(dataPath, json);
    }
}
