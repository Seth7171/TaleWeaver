// Filename: PageFlipper.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class manages the page flipping mechanism in the interactive book environment. It controls the state of adventure (ADV) objects
// and handles page transitions and mechanics using the EndlessBook and HandBookController systems.

using echo17.EndlessBook;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TaleWeaver.Gameplay;

/// <summary>
/// Manages the page flipping and adventure (ADV) object transitions within the book.
/// It listens for the end of an encounter and flips to the next page while enabling the appropriate ADV object.
/// </summary>
public class PageFlipper : MonoBehaviour
{
    // Singleton instance for global access to the PageFlipper
    public static PageFlipper Instance { get; private set; }

    // References to the EndlessBook and HandBookController
    public EndlessBook book;
    public HandBookController handBookController;

    // List of adventure objects that correspond to encounters or scenes in the book
    public List<GameObject> advObjects;

    // Index to track the current adventure object
    private int currentAdvIndex = 0;

    /// <summary>
    /// Cleans up the singleton instance and event listeners when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (OpenAIInterface.Instance != null)
                OpenAIInterface.Instance.OnIsEndedChanged -= OnIsEndedChanged;
            Debug.Log("PageFlipper instance is being destroyed.");
            Instance = null;
        }
    }

    /// <summary>
    /// Ensures that only one instance of PageFlipper exists across scenes (singleton pattern).
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Subscribes to the sceneLoaded event when the object is enabled.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event to prevent memory leaks when the object is disabled.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Handles actions when a new scene is loaded. If the MainMenu scene is loaded, it destroys the PageFlipper instance.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The mode of the scene load.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes the ADV objects and subscribes to OpenAIInterface's OnIsEndedChanged event.
    /// </summary>
    void Start()
    {
        // Disable all ADV objects initially
        foreach (var adv in advObjects)
        {
            adv.SetActive(false);
        }

        // Enable the first ADV object
        if (advObjects.Count > 0)
        {
            advObjects[0].SetActive(true);
        }

        // Subscribe to the event that triggers when an encounter ends
        OpenAIInterface.Instance.OnIsEndedChanged += OnIsEndedChanged;
    }

    /// <summary>
    /// Called when the OnIsEndedChanged event is triggered. Handles the flipping of the book pages and updating the handBookController.
    /// </summary>
    /// <param name="isEnded">Indicates whether the encounter has ended.</param>
    private void OnIsEndedChanged(bool isEnded)
    {
        if (isEnded)
        {
            string mechanic = GameMechanicsManager.Instance.currentMechnism;
            EnableNextAdv(mechanic);

            // Flip to the next page in the book
            book.TurnToPage(book.CurrentLeftPageNumber + 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
            
            // Update the handBookController state
            handBookController = FindObjectOfType<HandBookController>();
            if (!handBookController.is_readMode)
            {
                handBookController.HideEncounterOptions();
            }
        }
    }

    /// <summary>
    /// Enables the next ADV object in the list and prepares the next encounter mechanic.
    /// </summary>
    /// <param name="mechanic">The current encounter mechanic to set for the next ADV.</param>
    private void EnableNextAdv(string mechanic)
    {
        if (advObjects[currentAdvIndex] == null)
            return;

        if (currentAdvIndex < advObjects.Count - 1)
        {
            currentAdvIndex++;
            advObjects[currentAdvIndex].SetActive(true);
        }
    }
}
