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
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }
}
