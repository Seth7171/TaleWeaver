using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using echo17.EndlessBook;
using System.Collections.Generic;

public class ClickableTextUI : MonoBehaviour
{
    public UnityEvent onClick;
    public EndlessBook book;
    public List<GameObject> advObjects;
    private int currentAdvIndex = 0;

    private Dictionary<GameObject, TextMeshProUGUI> textObjects;

    private Color originalColor;
    public Color hoverColor = new Color(0.5f, 0.0f, 0.0f); // Bordo color

    void Start()
    {
        // Initialize dictionary for text objects
        textObjects = new Dictionary<GameObject, TextMeshProUGUI>();

        // Find all child TextMeshProUGUI components and add event listeners
        foreach (Transform child in transform)
        {
            var textMeshPro = child.GetComponent<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textObjects.Add(child.gameObject, textMeshPro);
                var eventTrigger = child.gameObject.AddComponent<EventTrigger>();

                AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, (data) => OnPointerEnter((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, (data) => OnPointerExit((PointerEventData)data, textMeshPro));
                AddEventTrigger(eventTrigger, EventTriggerType.PointerClick, (data) => OnPointerClick((PointerEventData)data, textMeshPro));
            }
        }

        // Disable all ADV objects initially
        foreach (var adv in advObjects)
        {
            adv.SetActive(false);
        }

        // Enable the first ADV object
        if (advObjects.Count > 0)
        {
            advObjects[0].SetActive(true);
        }

        OpenAIInterface.Instance.OnIsEndedChanged += OnIsEndedChanged;
    }

    private void OnDestroy()
    {
        OpenAIInterface.Instance.OnIsEndedChanged -= OnIsEndedChanged;
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

    private void OnPointerClick(PointerEventData eventData, TextMeshProUGUI textMeshPro)
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }

        // Get the first character of the TextMeshPro text
        string narrative = textMeshPro.text.Substring(0, 1);
        // Get the current book name from the OpenAIInterface instance
        string bookName = OpenAIInterface.Instance.current_BookName;
        // Call SendMessageToExistingBook with the book name, narrative (using first character of the TextMeshPro)
        OpenAIInterface.Instance.SendMessageToExistingBook(bookName, narrative);
    }

    private void OnIsEndedChanged(bool isEnded)
    {
        if (isEnded)
        {
            EnableNextAdv();
            book.TurnToPage(book.CurrentLeftPageNumber + 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
        }
    }

    private void EnableNextAdv()
    {
        if (currentAdvIndex < advObjects.Count - 1)
        {
            currentAdvIndex++;
            advObjects[currentAdvIndex].SetActive(true);
        }
    }
}
