using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Required for using TextMeshPro
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TrophyFadeManager : MonoBehaviour
{
    public Image trophiesPanelBackground;          // The background image of the TrophiesPanel
    public TMP_Text mainPanelText;                 // The main panel title text (e.g., "Trophies")
    public List<TMP_Text> trophyTexts;             // List of TMP_Text components for each trophy (TextMeshPro)
    public List<Image> trophyImages;               // List of regular trophy images
    public List<Image> trophyDisabledImages;       // List of disabled trophy images

    public float fadeDuration = 1f;                // Duration for each fade transition

    public static TrophyFadeManager Instance { get; internal set; }

    // Dictionary to map trophy text to index (reverse mapping from TrophiesManager)
    private Dictionary<string, int> trophyTextToIndex = new Dictionary<string, int>
    {
        { "WonAdventure", 0 },
        { "DieAdventure", 1 },
        { "Completed1Adventure", 2 },
        { "Completed5Adventures", 3 },
        { "Completed10Adventures", 4 },
        { "RolledCriticalPass", 5 },
        { "RolledCriticalSuccess", 6 }
    };

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("TrophyFadeManager instance is being destroyed.");
            Instance = null;
        }
    }

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set the initial state (fully transparent for all elements)
        trophiesPanelBackground.canvasRenderer.SetAlpha(0f);  // Ensure the panel background starts transparent
        mainPanelText.alpha = 0f;                             // Ensure the main panel text starts transparent

        foreach (var text in trophyTexts)
        {
            text.alpha = 0f;  // Ensure all TMP texts start transparent
        }

        foreach (var img in trophyImages)
        {
            img.canvasRenderer.SetAlpha(0f);   // Ensure all regular images start transparent
        }

        foreach (var disImg in trophyDisabledImages)
        {
            disImg.canvasRenderer.SetAlpha(0f);  // Ensure all disabled images start transparent
        }
    }

    // Method to reveal a specific trophy panel with fade effects
    public void RevealTrophyPanel(string trophyText)
    {
        if (trophyTextToIndex.ContainsKey(trophyText))
        {
            int index = trophyTextToIndex[trophyText];
            StartCoroutine(FadeTrophyPanel(index));
        }
        else
        {
            Debug.LogWarning("Trophy text not found in dictionary: " + trophyText);
        }
    }

    private IEnumerator FadeTrophyPanel(int index)
    {
        // Step 1: Fade in the panel background and main panel text
        StartCoroutine(FadeInImage(trophiesPanelBackground, fadeDuration));
        StartCoroutine(FadeInTMPText(mainPanelText, fadeDuration));

        // Fade in the trophy text, regular image, and disabled image
        StartCoroutine(FadeInTMPText(trophyTexts[index], fadeDuration));
        StartCoroutine(FadeInImage(trophyImages[index], fadeDuration));
        StartCoroutine(FadeInImage(trophyDisabledImages[index], fadeDuration));

        // Wait for the fade-in to complete
        yield return new WaitForSeconds(fadeDuration);

        // Step 2: Fade out the disabled image
        StartCoroutine(FadeOutImage(trophyDisabledImages[index], fadeDuration));

        // Wait for the fade-out to complete
        yield return new WaitForSeconds(fadeDuration);

        // Step 3: Fade out the panel background, main panel text, trophy text, and regular image
        StartCoroutine(FadeOutImage(trophiesPanelBackground, fadeDuration));
        StartCoroutine(FadeOutTMPText(mainPanelText, fadeDuration));
        StartCoroutine(FadeOutTMPText(trophyTexts[index], fadeDuration));
        StartCoroutine(FadeOutImage(trophyImages[index], fadeDuration));
    }

    // Helper function to fade in an Image component
    private IEnumerator FadeInImage(Image image, float duration)
    {
        float startAlpha = image.canvasRenderer.GetAlpha();
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            image.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, 1, time / duration));
            yield return null;
        }

        image.canvasRenderer.SetAlpha(1);  // Ensure it's fully visible at the end
    }

    // Helper function to fade out an Image component
    private IEnumerator FadeOutImage(Image image, float duration)
    {
        float startAlpha = image.canvasRenderer.GetAlpha();
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            image.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, 0, time / duration));
            yield return null;
        }

        image.canvasRenderer.SetAlpha(0);  // Ensure it's fully hidden at the end
    }

    // Helper function to fade in a TMP_Text component
    private IEnumerator FadeInTMPText(TMP_Text tmpText, float duration, bool isMainText = false)
    {
        float startAlpha = tmpText.alpha;
        float time = 0;
        float finishAlpha = 1;

        if (isMainText)
            finishAlpha = 0.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            tmpText.alpha = Mathf.Lerp(startAlpha, finishAlpha, time / duration);
            yield return null;
        }

        tmpText.alpha = finishAlpha;  // Ensure it's fully visible at the end
    }

    // Helper function to fade out a TMP_Text component
    private IEnumerator FadeOutTMPText(TMP_Text tmpText, float duration)
    {
        float startAlpha = tmpText.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            tmpText.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }

        tmpText.alpha = 0;  // Ensure it's fully hidden at the end
    }
}
