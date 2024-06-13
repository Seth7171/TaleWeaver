using System.Collections.Generic;
using System;

[Serializable]
public class PlayerData
{
    public List<Player> Players;

    public PlayerData()
    {
        Players = new List<Player>();
    }

    public bool AddPlayer(Player player)
    {
        if (Players.Exists(p => p.PlayerName == player.PlayerName))
        {
            return false; // Player already exists
        }
        Players.Add(player);
        return true;
    }

    public bool DeletePlayer(string playerName)
    {
        Player player = Players.Find(p => p.PlayerName == playerName);
        if (player != null)
        {
            Players.Remove(player);
            return true;
        }
        return false; // Player not found
    }
}
