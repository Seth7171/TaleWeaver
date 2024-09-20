// Filename: PlayerInGame.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This singleton script manages player health, luck, and game events such as death and victory. It handles audio cues, UI updates, and interactions with the OpenAI API.

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]


public class PlayerInGame : MonoBehaviour
{

    public int maxHealth = 14; // Maximum health of the player
    public int currentHealth; // Current health of the player
    public int maxLuck = 5; // Maximum luck of the player
    public int currentLuck; // Current luck of the player
    public int currentSkillModifier; // Current skill modifier

    public GameObject DeathScreen; // UI element for the death screen
    public TextMeshProUGUI DeathBackToMainMenu; // Text for back to main menu on death
    public TextMeshProUGUI DeathLoading; // Loading text for death
    public GameObject redScreen; // Red screen effect on damage

    public GameObject VictoryScreen; // UI element for the victory screen
    public TextMeshProUGUI VictoryBackToMainMenu; // Text for back to main menu on victory
    public TextMeshProUGUI VictoryLoading; // Loading text for victory

    public TMP_Text gender; // Gender of the player

    public HandBookController handBookController; // Reference to the handbook controller
    public GameObject decisions_Canvas; // UI canvas for decisions

    public AudioClip TakeDamageChosen; // Audio clip for taking damage
    private AudioClip DeathChosen; // Audio clip for death
    public AudioClip VictorySound; // Audio clip for victory
    private string sceneName; // Name of the current scene

    [SerializeField] AudioClip TakeDamageMale; // Audio clip for male taking damage
    [SerializeField] AudioClip DeathMale; // Audio clip for male death
    [SerializeField] AudioClip TakeDamageFemale; // Audio clip for female taking damage
    [SerializeField] AudioClip DeathFemale; // Audio clip for female death
    public AudioSource audioSource; // Audio source component

    public static PlayerInGame Instance { get; internal set; }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (OpenAIInterface.Instance != null)
            {
                OpenAIInterface.Instance.OnConclusionSave -= SaveDeathConclusionFinished;
                OpenAIInterface.Instance.OnConclusionSave -= SaveVictoryConclusionFinished;
            }
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("PlayerInGame instance is being destroyed.");
            Instance = null;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentHealth = 10;
            currentLuck = 2;
            currentSkillModifier = 0;
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
    /// Handles actions that occur when a new scene is loaded.
    /// </summary>
    /// <param name="scene">The scene that has been loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name; // Get the name of the current scene
        if (sceneName != "ViewPrevAdv")
        {
            GetGender(); // Get the player's gender
            audioSource = GetComponent<AudioSource>(); // Get the audio source component
        }

        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.OnConclusionSave += SaveDeathConclusionFinished;
            OpenAIInterface.Instance.OnConclusionSave += SaveVictoryConclusionFinished;
        }
        else
        {
            Debug.Log("OpenAIInterface is not initialize");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*        if (Input.GetKeyDown(KeyCode.Space))
                {
                    LoseLife(1);
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    GainLife(1);
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    LoseSkillModifier(1);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    GainSkillModifier(1);
                }*/
    }

    /// <summary>
    /// Increases the player's health by a specified amount.
    /// </summary>
    /// <param name="life">The amount of health to gain.</param>
    public void GainLife(int life)
    {
        currentHealth += life;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HealthBar.Instance.SetHealth(currentHealth);
    }

    /// <summary>
    /// Decreases the player's health by a specified amount.
    /// </summary>
    /// <param name="life">The amount of health to lose.</param>
    public void LoseLife(int life)
    {
        currentHealth -= life;
        HealthBar.Instance.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
        else
        {
            StartCoroutine(ShowRedScreen());
            audioSource.PlayOneShot(TakeDamageChosen);
        }
    }

    /// <summary>
    /// Displays a red screen effect briefly.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator ShowRedScreen()
    {
        redScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        redScreen.SetActive(false);
    }

    /// <summary>
    /// Increases the player's luck by a specified amount.
    /// </summary>
    /// <param name="luck">The amount of luck to gain.</param>
    public void GainLuck(int luck)
    {
        currentLuck += luck;
        if (currentLuck > maxLuck)
            currentLuck = maxLuck;
        LuckBar.Instance.SetLuck(currentLuck);
    }

    /// <summary>
    /// Decreases the player's luck by a specified amount.
    /// </summary>
    /// <param name="luck">The amount of luck to lose.</param>
    public void LoseLuck(int luck)
    {
        currentLuck -= luck;
        if (currentLuck < 0)
            currentLuck = 0;
        LuckBar.Instance.SetLuck(currentLuck);
    }

    /// <summary>
    /// Increases the player's skill modifier by a specified amount.
    /// </summary>
    /// <param name="modifier">The amount of skill modifier to gain.</param>
    public void GainSkillModifier(int modifier)
    {
        currentSkillModifier += modifier;
        ModifierNum.Instance.SetCheckModifier(currentSkillModifier);
    }

    /// <summary>
    /// Decreases the player's skill modifier by a specified amount.
    /// </summary>
    /// <param name="modifier">The amount of skill modifier to lose.</param>
    public void LoseSkillModifier(int modifier)
    {
        currentSkillModifier -= modifier;
        ModifierNum.Instance.SetCheckModifier(currentSkillModifier);
    }

    /// <summary>
    /// Handles player death, displaying the death screen and stopping player controls.
    /// </summary>
    void PlayerDeath()
    {
        Debug.Log("You Just Died ! RIP");
        audioSource.PlayOneShot(DeathChosen);
        decisions_Canvas.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
        handBookController.DisableControls();
        handBookController.is_scroll_lock = true;
        DeathScreen.SetActive(true);

        //need to move from here to when the API finished the conclution page!
        OpenAIInterface.Instance.SendMessageToExistingBook(PlayerSession.SelectedBookName, "player has died", 11);
        //SaveDeathConclusionFinished();
    }


    /// <summary>
    /// Handles player victory, displaying the victory screen and stopping player controls.
    /// </summary>
    public void SaveDeathConclusionFinished(bool isConcSaved)
    {
        if (isConcSaved && currentHealth <= 0)
        {
            TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { };
            TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { DeathLoading };
            TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { DeathBackToMainMenu };
            TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { DeathBackToMainMenu };
            TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { DeathLoading };
            ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        }

    }

    /// <summary>
    /// Handles player victory, displaying the victory screen and stopping player controls.
    /// </summary>
    public void PlayerVictory()
    {
        Debug.Log("You WON !");
        audioSource.PlayOneShot(VictorySound);
        decisions_Canvas.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
        handBookController.DisableControls();
        handBookController.is_scroll_lock = true;
        VictoryScreen.SetActive(true);

        //need to move from here to when the API finished the conclution page!
        OpenAIInterface.Instance.SendMessageToExistingBook(PlayerSession.SelectedBookName, "combat won, generate conclusion");
    }

    /// <summary>
    /// Handles the completion of saving the victory conclusion.
    /// </summary>
    /// <param name="isConcSaved">Indicates whether the conclusion was saved successfully.</param>
    public void SaveVictoryConclusionFinished(bool isConcSaved)
    {
        if (isConcSaved && currentHealth > 0)
        {
            TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { };
            TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { VictoryLoading };
            TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { VictoryBackToMainMenu };
            TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { VictoryBackToMainMenu };
            TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { VictoryLoading };
            ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        }

    }

    /// <summary>
    /// Loads the main menu scene and destroys the player instance.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }

    /// <summary>
    /// Determines the player's gender and assigns appropriate audio clips.
    /// </summary>
    public void GetGender()
    {
        if (gender.text == "Male")
        {
            TakeDamageChosen = TakeDamageMale;
            DeathChosen = DeathMale;
        }
        else
        {
            TakeDamageChosen = TakeDamageFemale;
            DeathChosen = DeathFemale;
        }
    }

}