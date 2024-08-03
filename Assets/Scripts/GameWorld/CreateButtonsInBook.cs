using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using echo17.EndlessBook;
using System.Collections.Generic;

public class CreateButtonsInBook : MonoBehaviour
{
    public UnityEvent onClick;

    private Dictionary<GameObject, TextMeshProUGUI> textObjects;

    private Color originalColor;

    public Color hoverColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color

    public void initialize(List<Option> mechnismOptions)
    {
        // Initialize dictionary for text objects
        textObjects = new Dictionary<GameObject, TextMeshProUGUI>();

        int optionIndx = 0;
        // Find all child TextMeshProUGUI components and add event listeners
        foreach (Transform child in transform)
        {
            var textMeshPro = child.GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textObjects.Add(child.gameObject, textMeshPro);
                var eventTrigger = child.gameObject.AddComponent<EventTrigger>();
                int currOptionIndx = mechnismOptions.Count <= optionIndx ? 0 : optionIndx;
                AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, (data) => OnPointerEnter((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, (data) => OnPointerExit((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerClick, (data) => OnPointerClick((PointerEventData)data, textMeshPro, mechnismOptions[currOptionIndx]));
                optionIndx++;
            }
        } 
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void OnPointerEnter(PointerEventData eventData, TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor;
        }
    }

    private void OnPointerExit(PointerEventData eventData, TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor;
        }
    }

    private void OnPointerClick(PointerEventData eventData, TextMeshProUGUI textMeshPro, Option mechnismOption)
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }

        // Get the option that will indicate the Mechanic of the choice
        string choice = textMeshPro.text;
        // Get the current book name from the OpenAIInterface instance
        string bookName = BookLoader.Instance.currentbookData.Name;
        // Call SendMessageToExistingBook with the book name, narrative (using first character of the TextMeshPro)

        GameMechanicsManager.Instance.HandlePlayerChoice(bookName, choice, mechnismOption);
    }
   
}
