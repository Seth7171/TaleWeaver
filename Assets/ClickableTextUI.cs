using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ClickableTextUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent onClick;
    private TextMeshProUGUI textMeshPro;

    private Color originalColor;
    public Color hoverColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        originalColor = textMeshPro.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        {
            if (onClick != null)
                if (onClick != null)
                {
                    {
                        onClick.Invoke();
                        onClick.Invoke();
                    }
                }
            // Get the first character of the TextMeshPro text
            string narrative = textMeshPro.text.Substring(0, 1);
            // Get the current book name from the OpenAIInterface instance
            string bookName = OpenAIInterface.Instance.current_BookName;
            // Call SendMessageToExistingBook with the book name, narrative, and page number (using first character of the TextMeshPro)
            OpenAIInterface.Instance.SendMessageToExistingBook(bookName, narrative, narrative);
        }
    }
}
