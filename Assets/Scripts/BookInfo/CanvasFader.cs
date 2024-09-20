// Filename: CanvasFader.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class provides functionality for fading in UI canvases using a fade effect. It ensures a smooth transition for canvases
// by adjusting their transparency over a defined duration, implemented using Unity's CanvasGroup component.

using UnityEngine;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

/// <summary>
/// Singleton class responsible for handling the fade-in effect of UI canvases in Unity.
/// It ensures smooth transitions by controlling the alpha value of CanvasGroup components over a set duration.
/// </summary>
public class CanvasFader : MonoBehaviour
{
    // Singleton instance to allow global access to the CanvasFader functionality
    public static CanvasFader Instance { get; internal set; }

    // Duration of the fade-in effect
    public float fadeDuration = 2.0f;

    /// <summary>
    /// Destroys the singleton instance when the GameObject is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log("CanvasFader instance is being destroyed.");
            Instance = null;
        }
    }

    /// <summary>
    /// Ensures that only one instance of the CanvasFader exists across scenes (singleton pattern).
    /// </summary>
    private void Awake()
    {
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

    /// <summary>
    /// Initializes the Canvas by setting its transparency to 0 (completely transparent) using a CanvasGroup component.
    /// </summary>
    /// <param name="canvasObject">The GameObject containing the Canvas to be initialized.</param>
    public void InitializeCanvas(GameObject canvasObject)
    {
        if (canvasObject == null)
        {
            Debug.LogWarning("Canvas GameObject is not assigned.");
            return;
        }

        // Ensure the Canvas starts transparent using a CanvasGroup component
        CanvasGroup canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        }

        // Set the initial alpha to 0 (transparent)
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Fades in the specified Canvas by gradually increasing its transparency over the set duration.
    /// </summary>
    /// <param name="canvasObject">The GameObject containing the Canvas to fade in.</param>
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

    /// <summary>
    /// Coroutine to handle the fade-in effect by gradually increasing the CanvasGroup's alpha value.
    /// </summary>
    /// <param name="canvasGroup">The CanvasGroup component whose transparency is adjusted.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
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
