// Filename: CreateButtonsInBook.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script creates interactive buttons in a book interface using TextMeshPro text. It handles hover effects and click events, passing the selected option to the GameMechanicsManager.

using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using echo17.EndlessBook;
using System.Collections.Generic;
using TaleWeaver.Gameplay;

public class CreateButtonsInBook : MonoBehaviour
{
    public UnityEvent onClick; // Event to invoke on button click

    private Dictionary<GameObject, TextMeshProUGUI> textObjects; // Dictionary to map GameObjects to TextMeshPro components

    private Color originalColor; // Store the original color of the text

    public Color hoverColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color for hover effect

    /// <summary>
    /// Initializes the buttons with the given mechanism options and sets up event listeners.
    /// </summary>
    /// <param name="mechnismOptions">List of options for button actions.</param>
    public void initialize(List<Option> mechnismOptions)
    {
        // Initialize dictionary for text objects
        textObjects = new Dictionary<GameObject, TextMeshProUGUI>();

        int optionIndx = 0; // Index for the mechanism options

        // Find all child TextMeshProUGUI components and add event listeners
        foreach (Transform child in transform)
        {
            var textMeshPro = child.GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textObjects.Add(child.gameObject, textMeshPro); // Add to dictionary
                var eventTrigger = child.gameObject.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.triggers.Clear(); // Clear existing triggers
                }
                else
                {
                    // Add a new EventTrigger component if none exists
                    eventTrigger = child.gameObject.AddComponent<EventTrigger>();
                }
                // Set the current option index based on the count of mechanism options
                int currOptionIndx = mechnismOptions.Count <= optionIndx ? 0 : optionIndx;
                // Add event triggers for pointer enter, exit, and click
                AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, (data) => OnPointerEnter((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, (data) => OnPointerExit((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerClick, (data) => OnPointerClick((PointerEventData)data, textMeshPro, mechnismOptions[currOptionIndx]));
                optionIndx++; // Increment the option index
            }
        }
    }

    /// <summary>
    /// Adds an event trigger to the specified EventTrigger component.
    /// </summary>
    /// <param name="trigger">The EventTrigger to add to.</param>
    /// <param name="eventType">The type of event to trigger.</param>
    /// <param name="action">The action to invoke on the event.</param>
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType }; // Create a new entry for the event
        entry.callback.AddListener(action); // Add the action to the entry
        trigger.triggers.Add(entry); // Add the entry to the trigger
    }

    /// <summary>
    /// Changes the text color to hoverColor when the pointer enters the text area.
    /// </summary>
    private void OnPointerEnter(PointerEventData eventData, TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor; // Change color to hover color
        }
    }

    /// <summary>
    /// Resets the text color to its original color when the pointer exits the text area.
    /// </summary>
    private void OnPointerExit(PointerEventData eventData, TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor; // Reset to original color
        }
    }

    /// <summary>
    /// Invokes the onClick event and handles the player's choice when the text is clicked.
    /// </summary>
    private void OnPointerClick(PointerEventData eventData, TextMeshProUGUI textMeshPro, Option mechnismOption)
    {
        if (onClick != null)
        {
            onClick.Invoke(); // Invoke the onClick event
        }

        // Get the option that indicates the mechanic of the choice
        string choice = textMeshPro.text;
        // Get the current book name from the OpenAIInterface instance
        string bookName = BookLoader.Instance.currentbookData.Name;
        // Call HandlePlayerChoice with the book name and choice
        GameMechanicsManager.Instance.HandlePlayerChoice(bookName, choice, mechnismOption);
    }
}
