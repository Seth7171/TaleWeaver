// Filename: ClickableText.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script allows TextMeshPro text to be clickable. It changes color on hover and invokes an event when clicked. It also ensures the correct collider size for interaction.
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

    /// <summary>
    /// Initializes the text, stores its original color, and ensures it has a correctly sized collider.
    /// </summary>
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

    /// <summary>
    /// Updates the BoxCollider size to match the dimensions of the TextMeshPro text.
    /// </summary>
    void UpdateCollider()
    {
        var collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(textMeshPro.preferredWidth, textMeshPro.preferredHeight, 0.01f);
        collider.center = new Vector3(textMeshPro.preferredWidth / 2, -textMeshPro.preferredHeight / 2, 0);

        Debug.Log("Collider Size: " + collider.size);
        Debug.Log("Collider Center: " + collider.center);
    }

    /// <summary>
    /// Invokes the onClick event when the text is clicked.
    /// </summary>
    void OnMouseDown()
    {
        Debug.Log("Text clicked!");
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }

    /// <summary>
    /// Changes the text color to hoverColor when the mouse enters the text area.
    /// </summary>
    void OnMouseEnter()
    {
        Debug.Log("Mouse Entered");
        textMeshPro.color = hoverColor;
    }

    /// <summary>
    /// Resets the text color to its original color when the mouse exits the text area.
    /// </summary>
    void OnMouseExit()
    {
        Debug.Log("Mouse Exited");
        textMeshPro.color = originalColor;
    }

    /// <summary>
    /// Updates the collider if the text size changes in the editor.
    /// </summary>
    void OnValidate()
    {
        if (textMeshPro != null)
        {
            UpdateCollider();
        }
    }
}
