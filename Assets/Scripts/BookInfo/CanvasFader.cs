using UnityEngine;

public class CanvasFader : MonoBehaviour
{
    public static CanvasFader Instance { get; private set; }

    public float fadeDuration = 2.0f; // Duration of the fade-in effect

    private void Awake()
    {
        // Singleton pattern to ensure a single instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("CanvasFader instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeCanvas(GameObject canvasObject)
    {
        if (canvasObject == null)
        {
            Debug.LogWarning("Canvas GameObject is not assigned.");
            return;
        }

        // Ensure the Canvas starts transparent
        CanvasGroup canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        }

        // Set the initial alpha to 0 (transparent)
        canvasGroup.alpha = 0f;
    }

    public void FadeInCanvas(GameObject canvasObject)
    {
        if (canvasObject == null)
        {
            Debug.LogWarning("Canvas GameObject is not assigned.");
            return;
        }

        // Get or add a CanvasGroup component to the GameObject
        CanvasGroup canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        }

        // Start the fade-in process
        StartCoroutine(FadeInCoroutine(canvasGroup));
    }

    private System.Collections.IEnumerator FadeInCoroutine(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;

        // Start fading from transparent
        canvasGroup.alpha = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null; // Wait for the next frame
        }

        // Ensure it's fully opaque at the end
        canvasGroup.alpha = 1f;
    }
}
