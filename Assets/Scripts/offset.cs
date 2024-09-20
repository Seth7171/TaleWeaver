// Filename: AdjustButtonPosition.cs
// Author: Nitsan Maman & Ron Shahar
// Created on: 15/07/2024
// Description: This class adjusts the button position and handles pointer events to ensure they are registered correctly within a defined clickable area.

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Adjusts the button position and handles pointer events to ensure they are registered correctly within a defined clickable area.
/// </summary>
public class AdjustButtonPosition : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform buttonRectTransform; // The RectTransform of the button
    public RectTransform clickableArea; // The area within which clicks are registered
    public Camera secondaryCamera; // Camera used to convert screen points
    public Vector2 offset; // Offset to adjust the clickable area

    void Start()
    {
        // Initialize the button's RectTransform if not assigned
        if (buttonRectTransform == null)
        {
            buttonRectTransform = GetComponent<RectTransform>();
        }

        // Use the button's RectTransform as the clickable area if none is assigned
        if (clickableArea == null)
        {
            clickableArea = buttonRectTransform;
        }

        // Adjust the clickable area RectTransform settings
        AdjustClickableAreaRect();
    }

    /// <summary>
    /// Handles pointer click events.
    /// </summary>
    /// <param name="eventData">The data associated with the pointer event.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }
    }

    /// <summary>
    /// Handles pointer down events.
    /// </summary>
    /// <param name="eventData">The data associated with the pointer event.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }
    }

    /// <summary>
    /// Handles pointer up events.
    /// </summary>
    /// <param name="eventData">The data associated with the pointer event.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }
    }

    /// <summary>
    /// Checks if the pointer is over the clickable area.
    /// </summary>
    /// <param name="eventData">The data associated with the pointer event.</param>
    /// <returns>True if the pointer is within the clickable area; otherwise, false.</returns>
    private bool IsPointerOverClickableArea(PointerEventData eventData)
    {
        if (clickableArea == null)
        {
            return false;
        }

        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(clickableArea, eventData.position, secondaryCamera, out localMousePosition);

        // Apply the offset to the local mouse position
        localMousePosition += offset;

        Debug.Log($"Local Mouse Position: {localMousePosition}");
        Debug.Log($"Clickable Area Rect: {clickableArea.rect}");

        return clickableArea.rect.Contains(localMousePosition);
    }

    /// <summary>
    /// Adjusts the clickable area RectTransform to start at (0, 0).
    /// </summary>
    private void AdjustClickableAreaRect()
    {
        clickableArea.offsetMin = Vector2.zero;
        clickableArea.offsetMax = new Vector2(clickableArea.rect.width, clickableArea.rect.height);
    }
}
