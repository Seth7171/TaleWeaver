using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            Debug.Log("ButtonFader instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            float time = Time.deltaTime;
            if (time == 0)
                time = 0.017f;
            for (float t = 0.01f; t < fadeDuration; t += time)
            {
                foreach (TextMeshProUGUI text in texts)
                    text.color = Color.Lerp(Color.clear, originalColor, Mathf.Min(1, t / fadeDuration));
                yield return null;
            }
            foreach (TextMeshProUGUI text in texts)
                text.color = originalColor;
        }
    }

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
