// Filename: BookmarkClickHandler.cs
// Author: Nitsan Maman & Ron Shahar
// Description: Handles mouse click interactions on bookmarks within a scene, detecting clicks on bookmark objects 
// and triggering responses using raycasting from the main camera. This class supports interactive features in a 
// digital handbook or similar application.

using UnityEngine;

/// <summary>
/// Handles click interactions on bookmarks within a scene. Detects mouse clicks on bookmark objects and triggers appropriate responses.
/// This class is part of the user interface management for an interactive book application.
/// </summary>
public class BookmarkClickHandler : MonoBehaviour
{
    // Reference to the main camera in the scene to calculate raycasts from screen points.
    public Camera mainCamera;

    // Controller component that manages interactions and responses for handbook bookmarks.
    private HandBookController handBookController;

    /// <summary>
    /// Initializes the BookmarkClickHandler by finding the HandBookController component in the scene.
    /// Logs initialization for debugging purposes.
    /// </summary>
    private void Start()
    {
        handBookController = FindObjectOfType<HandBookController>();
        Debug.Log("BookmarkClickHandler Initialized");
    }

    /// <summary>
    /// Listens for mouse clicks and performs a raycast to detect if a bookmark has been clicked.
    /// Logs hit results and triggers actions via the HandBookController.
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Checks if the left mouse button was clicked.
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // Converts the mouse position into a ray.
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Bookmarks");  // Filters raycast to only consider objects in the "Bookmarks" layer.

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
                Debug.Log("Bookmark clicked: " + hit.transform.name);
                handBookController.OnBookmarkClicked(hit.transform);
            }
            else
            {
                Debug.Log("Raycast did not hit a bookmark.");
            }
        }
    }
}
