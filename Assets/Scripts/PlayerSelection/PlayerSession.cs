// Filename: PlayerSession.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Stores session data for the currently selected player and book.

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Static class that holds session information for the selected player and book.
/// </summary>
public static class PlayerSession
{
    public static string SelectedPlayerName { get; set; } // The name of the selected player
    public static string SelectedPlayerApiKey { get; set; } // The API key for the selected player
    public static string SelectedPlayerassistantID { get; set; } // The assistant ID for the selected player
    public static string SelectedBookName { get; set; } // The name of the currently selected book
}
