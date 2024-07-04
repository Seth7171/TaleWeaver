using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    public float startDelay = 0.1f; // Delay before the fade starts
    private Image image;
    private float currentTime;
    private bool isFading = false;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No Image component found on this GameObject.");
            return;
        }
        currentTime = fadeDuration;
        // Start the fade after the delay
        Invoke("StartFade", startDelay);
    }

    void StartFade()
    {
        isFading = true;
    }

    void Update()
    {
        if (isFading && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            float alpha = Mathf.Clamp01(currentTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}
