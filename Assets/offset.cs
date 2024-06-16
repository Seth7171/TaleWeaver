using UnityEngine;
using UnityEngine.EventSystems;

public class AdjustButtonPosition : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform buttonRectTransform;
    public RectTransform clickableArea;
    public Camera secondaryCamera;
    public Vector2 offset; // Offset to adjust the clickable area

    void Start()
    {
        if (buttonRectTransform == null)
        {
            buttonRectTransform = GetComponent<RectTransform>();
        }
        if (clickableArea == null)
        {
            clickableArea = buttonRectTransform;
        }
        AdjustClickableAreaRect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsPointerOverClickableArea(eventData))
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }
    }

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

    private void AdjustClickableAreaRect()
    {
        // Adjust the clickable area rect to start at (0, 0)
        clickableArea.offsetMin = Vector2.zero;
        clickableArea.offsetMax = new Vector2(clickableArea.rect.width, clickableArea.rect.height);
    }
}
