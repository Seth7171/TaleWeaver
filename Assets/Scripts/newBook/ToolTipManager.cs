// Filename: ToolTipManager.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages the display of tooltips in the UI, updating their position and content based on mouse events.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages tooltips displayed in the UI, allowing for dynamic content and positioning based on mouse interactions.
/// </summary>
public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
    public TextMeshProUGUI advText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
            instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Sets the tooltip message and makes the tooltip visible.
    /// </summary>
    /// <param name="message">The message to display in the tooltip.</param>
    public void SetAndShowToolTip(string message)
    {
        gameObject.SetActive(true);
        advText.text = message;
    }

    /// <summary>
    /// Hides the tooltip and clears the text.
    /// </summary>
    public void HideToolTip()
    {
        gameObject.SetActive(false);
        advText.text = string.Empty;
    }
}
