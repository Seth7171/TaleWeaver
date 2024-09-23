using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrophiesManager : MonoBehaviour
{
    [SerializeField] private Image[] disabledTrophyImages; // List of disabled (locked) images to cover the regular one

    // Dictionary to store trophy names and PlayerPrefs keys
    private Dictionary<int, string> trophyDictionary = new Dictionary<int, string>
    {
        { 0, "WonAdventure" },
        { 1, "DieAdventure" },
        { 2, "Completed1Adventure" },
        { 3, "Completed5Adventures" },
        { 4, "Completed10Adventures" },
        { 5, "RolledCriticalPass" },
        { 6, "RolledCriticalSuccess" }
    };

    void Start()
    {
/*        // Save the updated count back into PlayerPrefs
        PlayerPrefs.SetInt("AdventuresCompleted", 0);
        PlayerPrefs.SetInt("Completed1Adventure", 0);*/
        UpdateTrophyStatus();
    }

    void UpdateTrophyStatus()
    {
        for (int i = 0; i < disabledTrophyImages.Length; i++)
        {
            // Fetch the PlayerPrefs key for the current trophy from the dictionary
            bool trophyAchieved = PlayerPrefs.GetInt(trophyDictionary[i], 0) == 1;
            if (trophyAchieved)
            {
                // Show the trophy (unlocked) image by hiding the disabled one
                disabledTrophyImages[i].gameObject.SetActive(false);
            }
            else
            {
                // Hide the trophy (unlocked) image by UNhiding the disabled one
                disabledTrophyImages[i].gameObject.SetActive(true);
            }
        }
    }

    // Call this method when a trophy is achieved
    public void UnlockTrophy(int trophyIndex)
    {
        PlayerPrefs.SetInt("Trophy" + trophyIndex + "Achieved", 1);
        UpdateTrophyStatus();  // Refresh the trophy display
    }
}
