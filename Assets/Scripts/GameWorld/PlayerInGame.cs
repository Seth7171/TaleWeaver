using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInGame : MonoBehaviour
{

    public int maxHealth = 14;
    public int currentHealth;
    public int maxLuck = 5;
    public int currentLuck;
    public int currentSkillModifier;

    public GameObject DeathScreen;
    public TextMeshProUGUI DeathBackToMainMenu;
    public TextMeshProUGUI DeathLoading;
    public GameObject redScreen;

    public GameObject VictoryScreen;
    public TextMeshProUGUI VictoryBackToMainMenu;
    public TextMeshProUGUI VictoryLoading;

    public TMP_Text gender;

    public HandBookController handBookController;
    public GameObject decisions_Canvas;

    private AudioClip TakeDamageChosen;
    private AudioClip DeathChosen;
    public AudioClip VictorySound;
    private string sceneName;

    [SerializeField] AudioClip TakeDamageMale;
    [SerializeField] AudioClip DeathMale;
    [SerializeField] AudioClip TakeDamageFemale;
    [SerializeField] AudioClip DeathFemale;
    AudioSource audioSource;

    public static PlayerInGame Instance { get; private set; }

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
            //DontDestroyOnLoad(gameObject);
            currentHealth = 10;
            currentLuck = 2;
            currentSkillModifier = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "GameWorld")
        {
            GetGender();
            audioSource = GetComponent<AudioSource>();
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
 /*       if (Input.GetKeyDown(KeyCode.Space))
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

    public void GainLife(int life)
    {
        currentHealth += life;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HealthBar.Instance.SetHealth(currentHealth);
    }

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

    IEnumerator ShowRedScreen()
    {
        redScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        redScreen.SetActive(false);
    }

    public void GainLuck(int luck)
    {
        currentLuck += luck;
        if (currentLuck > maxLuck)
            currentLuck = maxLuck;
        LuckBar.Instance.SetLuck(currentLuck);
    }

    public void LoseLuck(int luck)
    {
        currentLuck -= luck;
        if (currentLuck < 0)
            currentLuck = 0;
        LuckBar.Instance.SetLuck(currentLuck);
    }

    public void GainSkillModifier(int modifier)
    {
        currentSkillModifier += modifier;
        ModifierNum.Instance.SetCheckModifier(currentSkillModifier);
    }

    public void LoseSkillModifier(int modifier)
    {
        currentSkillModifier -= modifier;
        ModifierNum.Instance.SetCheckModifier(currentSkillModifier);
    }

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



    public void SaveDeathConclusionFinished(bool isConcSaved)
    {
        if(isConcSaved && currentHealth <=0)
        {
            TextMeshProUGUI[] UICanvasDisable = new TextMeshProUGUI[] { };
            TextMeshProUGUI[] textToFade = new TextMeshProUGUI[] { DeathLoading };
            TextMeshProUGUI[] UICanvasEnable = new TextMeshProUGUI[] { DeathBackToMainMenu };
            TextMeshProUGUI[] textToReveal = new TextMeshProUGUI[] { DeathBackToMainMenu };
            TextMeshProUGUI[] textToDisable = new TextMeshProUGUI[] { DeathLoading };
            ButtonFader.Instance.Fader(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable);
        }

    }

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

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }


    public void GetGender()
    {
        if(gender.text == "Male")
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
