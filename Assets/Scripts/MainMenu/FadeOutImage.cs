using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    private Image image;
    private float currentTime;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No Image component found on this GameObject.");
            return;
        }
        currentTime = fadeDuration;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            float alpha = Mathf.Clamp01(currentTime / fadeDuration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}
