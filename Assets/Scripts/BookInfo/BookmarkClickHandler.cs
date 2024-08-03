using UnityEngine;

public class BookmarkClickHandler : MonoBehaviour
{
    public Camera mainCamera;

    private HandBookController handBookController;

    private void Start()
    {
        handBookController = FindObjectOfType<HandBookController>();
        Debug.Log("BookmarkClickHandler Initialized");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Bookmarks");

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
