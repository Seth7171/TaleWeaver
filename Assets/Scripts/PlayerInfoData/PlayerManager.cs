using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerManager
{
    public List<string> PlayerNames;

    public PlayerManager()
    {
        PlayerNames = new List<string>();
    }

    public bool AddPlayer(string playerName)
    {
        if (PlayerNames.Contains(playerName))
        {
            return false; // Player already exists
        }
        PlayerNames.Add(playerName);
        return true;
    }

    public bool DeletePlayer(string playerName)
    {
        return PlayerNames.Remove(playerName);
    }
}

