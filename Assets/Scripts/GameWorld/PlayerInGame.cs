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
        DeathScreen.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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
