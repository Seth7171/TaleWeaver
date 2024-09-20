// Filename: SceneMusicManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages background music for different scenes, allowing for random track selection and persistence across scenes.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages background music for the game scenes, including playing random tracks and handling transitions between scenes.
/// </summary>
public class SceneMusicManager : MonoBehaviour
{
    public AudioClip mainTheme; // Main theme music for the main menu
    public List<AudioClip> musicTracks; // List of available music tracks
    private AudioSource audioSource; // Reference to the AudioSource component
    private List<AudioClip> remainingTracks; // Tracks that are yet to be played
    public static SceneMusicManager Instance { get; private set; } // Singleton instance

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("SceneMusicManager instance is being destroyed.");
            Instance = null;
        }
    }

    /// <summary>
    /// Initializes the singleton instance and sets up the audio source.
    /// </summary>
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Handles actions when a new scene is loaded.
    /// </summary>
    /// <param name="scene">The scene that has been loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "MainMenu")
        {

            audioSource.clip = mainTheme; // Set main theme for the main menu
            audioSource.Stop(); // Stop any playing music

        }
        if (scene.name == "LoadingScene")
        {
            Destroy(gameObject); // Destroy the manager when loading scene is active
        }
        if (scene.name == "ViewPrevAdv")
        {
            Awake(); // Re-initialize the audio source
        }
    }



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        remainingTracks = new List<AudioClip>(musicTracks); // Clone the list of music tracks

        VolumeManager.Instance?.SetCurrentAudioSource(audioSource); // Set the current audio source in VolumeManager

        if (remainingTracks.Count > 0)
        {
            PlayRandomTrack(); // Play a random track if available
        }
    }

    /// <summary>
    /// Plays a random track from the remaining tracks list.
    /// </summary>
    private void PlayRandomTrack()
    {
        if (remainingTracks.Count == 0)
        {
            remainingTracks = new List<AudioClip>(musicTracks); // Reset if all tracks have been played
        }

        int randomIndex = Random.Range(0, remainingTracks.Count); // Select a random track
        audioSource.clip = remainingTracks[randomIndex]; // Set the audio clip
        audioSource.Play();

        remainingTracks.RemoveAt(randomIndex); // Remove the track from the list

        Invoke("PlayRandomTrack", audioSource.clip.length); // Schedule the next track
    }
}