using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonFader : MonoBehaviour
{
    public static ButtonFader Instance { get; private set; }
    public float fadeDuration = 1.0f;
    // Start is called before the first frame update
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
            Color originalColor = texts[0].color;
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
}
