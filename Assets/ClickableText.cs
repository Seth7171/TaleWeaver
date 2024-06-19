using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ClickableText : MonoBehaviour
{
    public UnityEvent onClick;
    private TextMeshProUGUI textMeshPro;

    private Color originalColor;
    public Color hoverColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        originalColor = textMeshPro.color;

        // Ensure the text has a collider
        if (GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        // Ensure the collider matches the text size
        UpdateCollider();

        // Check if the collider is correctly configured
        if (!GetComponent<Collider>().enabled)
        {
            Debug.LogWarning("Collider is not enabled!");
        }
    }

    void UpdateCollider()
    {
        var collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(textMeshPro.preferredWidth, textMeshPro.preferredHeight, 0.01f);
        collider.center = new Vector3(textMeshPro.preferredWidth / 2, -textMeshPro.preferredHeight / 2, 0);

        Debug.Log("Collider Size: " + collider.size);
        Debug.Log("Collider Center: " + collider.center);
    }

    void OnMouseDown()
    {
        Debug.Log("Text clicked!");
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse Entered");
        textMeshPro.color = hoverColor;
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse Exited");
        textMeshPro.color = originalColor;
    }

    void OnValidate()
    {
        if (textMeshPro != null)
        {
            UpdateCollider();
        }
    }
}
