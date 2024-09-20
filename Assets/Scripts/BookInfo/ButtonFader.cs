// Filename: ButtonFader.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class handles fading in and out of UI elements like TextMeshPro text fields and buttons,
// providing smooth visual transitions for game interfaces.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages fading transitions for UI elements like TextMeshPro text fields and buttons.
/// Handles both fade in and fade out effects as well as color transitions (e.g., to Bordo color).
/// </summary>
public class ButtonFader : MonoBehaviour
{
    public static ButtonFader Instance { get; private set; }
    public float fadeDuration = 1.0f;

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("Singleton instance is being destroyed.");
            Instance = null;
        }
    }


    /// <summary>
    /// Initializes the ButtonFader singleton instance and prevents its destruction across scene loads.
    /// </summary>
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ButtonFader instance initialized.");
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

    /// <summary>
    /// Initiates the fade transition for the specified TextMeshPro UI elements, fading out one set and revealing another.
    /// </summary>
    /// <param name="UICanvasDisable">Array of UI elements to be hidden.</param>
    /// <param name="textToFade">Array of text elements to be faded out.</param>
    /// <param name="UICanvasEnable">Array of UI elements to be shown.</param>
    /// <param name="textToReveal">Array of text elements to be revealed.</param>
    /// <param name="textToDisable">Array of text elements to be hidden.</param>
    public void Fader(TextMeshProUGUI[] UICanvasDisable, TextMeshProUGUI[] textToFade, TextMeshProUGUI[] UICanvasEnable, TextMeshProUGUI[] textToReveal, TextMeshProUGUI[] textToDisable)
    {
        StartCoroutine(FadeText(UICanvasDisable, textToFade, UICanvasEnable, textToReveal, textToDisable));
    }

    IEnumerator FadeText(TextMeshProUGUI[] UICanvasDisable, TextMeshProUGUI[] textToFade, TextMeshProUGUI[] UICanvasEnable, TextMeshProUGUI[] textToReveal, TextMeshProUGUI[] textToDisable)
    {
        // Fade out UICanvasFade
        foreach (TextMeshProUGUI canvas in UICanvasDisable)
            canvas.gameObject.SetActive(false);

        // Fade out textToFade
        yield return StartCoroutine(FadeOutText(textToFade));

        // Reveal UICanvasReveal
        foreach (TextMeshProUGUI canvas in UICanvasEnable)
            canvas.gameObject.SetActive(true);

        // Reveal textToReveal
        yield return StartCoroutine(FadeInText(textToReveal));

        // Disable textToDisable
        foreach (TextMeshProUGUI texts in textToDisable)
            texts.gameObject.SetActive(false);

    }

    IEnumerator FadeOutText(TextMeshProUGUI[] texts)
    {
        if (texts.Length > 0)
        {
            Color originalColor = texts[0].color;
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (TextMeshProUGUI text in texts)
                    text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (TextMeshProUGUI text in texts)
                text.color = Color.clear;
        }

    }

    IEnumerator FadeInText(TextMeshProUGUI[] texts)
    {
        if (texts.Length > 0)
        {
            Color originalColor = Color.black;
            if (texts[0].text.Contains("Back to main menu"))
            {
                originalColor = Color.white;
            }
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (TextMeshProUGUI text in texts)
                    if (text.text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 5)
                    {
                        if (text.text.Contains("Lose") || text.text.Contains("+"))
                            text.color = Color.Lerp(Color.clear, Color.red, Mathf.Min(1, t / fadeDuration));
                        else if (text.text.Contains("Gain") || text.text.Contains("-"))
                            text.color = Color.Lerp(Color.clear, new Color(0.0f, 0.5f, 0.0f), Mathf.Min(1, t / fadeDuration));
                    }
                    else
                        text.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (TextMeshProUGUI text in texts)
                if (text.text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 5)
                {
                    if (text.text.Contains("Lose") || text.text.Contains("+"))
                        text.color = Color.red;
                    else if (text.text.Contains("Gain") || text.text.Contains("-"))
                        text.color = new Color(0.0f, 0.5f, 0.0f);
                }
                else
                    text.color = originalColor;
        }
    }

    /// <summary>
    /// Fades in or out the specified buttons based on the 'toReveal' parameter.
    /// </summary>
    /// <param name="buttonsToFade">Array of buttons to be faded in or out.</param>
    /// <param name="toReveal">Indicates whether to fade in (true) or fade out (false).</param>
    public void FadeButtons(Button[] buttonsToFade, bool toReveal)
    {
        if (toReveal)
        {
            // Fade in buttonsToFade
            StartCoroutine(FadeInButtons(buttonsToFade));

        }

        else
        {
            // Fade out buttonsToFade
            StartCoroutine(FadeOutButtons(buttonsToFade));
        }
    }

    IEnumerator FadeOutButtons(Button[] buttons)
    {
        if (buttons.Length > 0)
        {
            Color originalColor = buttons[0].image.color;
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (Button button in buttons)
                    if (button != null)
                        button.image.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.image.color = Color.clear;
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator FadeInButtons(Button[] buttons)
    {
        if (buttons.Length > 0)
        {
            foreach (Button button in buttons)
                if (button != null)
                    button.gameObject.SetActive(true);
            Color originalColor = Color.white;
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (Button button in buttons)
                    if (button != null)
                        button.image.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (Button button in buttons)
                if (button != null)
                    button.image.color = originalColor;
        }
    }

    /// <summary>
    /// Initiates the fade transition to the Bordo color for the specified TextMeshPro elements.
    /// </summary>
    /// <param name="texts">Array of TextMeshPro elements to be faded to Bordo color.</param>
    public void FaderBordo(TextMeshProUGUI[] texts)
    {
        StartCoroutine(FadeToBordo(texts));
    }

    IEnumerator FadeToBordo(TextMeshProUGUI[] texts)
    {
        if (texts.Length > 0)
        {
            Color originalColor = texts[0].color;
            Color BordoColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (TextMeshProUGUI text in texts)
                    text.color = Color.Lerp(originalColor, BordoColor, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (TextMeshProUGUI text in texts)
                text.color = BordoColor;
        }
    }
}