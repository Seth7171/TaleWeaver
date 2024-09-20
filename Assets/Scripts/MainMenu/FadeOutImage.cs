// Filename: FadeOutImage.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script fades out an Image component over a specified duration, with an optional start delay.

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles fading out an image component over a specified duration.
/// </summary>
public class FadeOutImage : MonoBehaviour
{
    // Fade settings
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    public float startDelay = 0.1f; // Delay before the fade starts

    private Image image; // Reference to the Image component
    private float currentTime; // Current time remaining for the fade
    private bool isFading = false; // Flag to check if fading is in progress

    /// <summary>
    /// Initializes the image component and starts the fade after a delay.
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>(); // Get the Image component
        if (image == null)
        {
            Debug.LogError("No Image component found on this GameObject."); // Log error if no Image component is found
            return;
        }
        currentTime = fadeDuration; // Set the current time to the fade duration
        Invoke("StartFade", startDelay); // Start the fade after the specified delay
    }

    /// <summary>
    /// Begins the fade process.
    /// </summary>
    void StartFade()
    {
        isFading = true; // Set the fading flag to true
    }

    /// <summary>
    /// Updates the image's alpha value based on the current time remaining.
    /// </summary>
    void Update()
    {
        if (isFading && currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Decrease the current time
            float alpha = Mathf.Clamp01(currentTime / fadeDuration); // Calculate the new alpha value
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha); // Set the image color with the new alpha
        }
    }
}
