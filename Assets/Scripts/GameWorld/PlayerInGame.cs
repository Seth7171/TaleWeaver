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

    public HealthBar healthBar;
    public LuckBar LuckBar;
    public GameObject DeathScreen;
    public TextMeshProUGUI DeathBackToMainMenu;
    public TextMeshProUGUI DeathLoading;
    public GameObject redScreen;
    public TMP_Text gender;

    public HandBookController handBookController;
    public GameObject decisions_Canvas;

    private AudioClip TakeDamageChosen;
    private AudioClip DeathChosen;

    [SerializeField] AudioClip TakeDamageMale;
    [SerializeField] AudioClip DeathMale;
    [SerializeField] AudioClip TakeDamageFemale;
    [SerializeField] AudioClip DeathFemale;
    AudioSource audioSource;

    public static PlayerInGame Instance { get; private set; }

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        GetGender();
        audioSource = GetComponent<AudioSource>();
        currentHealth = 10;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        currentLuck = 2;
        LuckBar.SetMaxLuck(maxLuck);
        LuckBar.SetLuck(currentLuck);

        if (OpenAIInterface.Instance != null)
        {
            OpenAIInterface.Instance.OnConclusionSave += SaveDeathConclusionFinished;
        }
        else
        {
            Debug.Log("OpenAIInterface is not initialize");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoseLife(1);
        }

        if (currentHealth == 0)
        {
            // end the adventure
        }
    }

    private void OnDestroy()
    {
        if (OpenAIInterface.Instance != null)
            OpenAIInterface.Instance.OnConclusionSave -= SaveDeathConclusionFinished;
    }

    public void GainLife(int life)
    {
        currentHealth += life;
        healthBar.SetHealth(currentHealth);
    }

    public void LoseLife(int life)
    {
        currentHealth -= life;
        healthBar.SetHealth(currentHealth);
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
        LuckBar.SetLuck(currentLuck);
    }

    public void LoseLuck(int luck)
    {
        currentLuck -= luck;
        LuckBar.SetLuck(currentLuck);
    }

    void PlayerDeath()
    {
        Debug.Log("You Just Died ! RIP");
        audioSource.PlayOneShot(DeathChosen);
        Time.timeScale = 0f;
        decisions_Canvas.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
        handBookController.DisableControls();
        handBookController.is_scroll_lock = true;
        DeathScreen.SetActive(true);

        //need to move from here to when the API finished the conclution page!
        OpenAIInterface.Instance.SendNarrativeToAPI(PlayerSession.SelectedBookName, "player has died", 11);
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
