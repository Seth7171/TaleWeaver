using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentureForthButton : MonoBehaviour
{
    public Button ventureForthButton;
    public TMP_InputField inputField1; // Reference to the first input field
    public TMP_InputField inputField2; // Reference to the second input field
    public TMP_Text notificationText; // Reference to a text component for notifications

    void Start()
    {
        if (ventureForthButton != null)
        {
            ventureForthButton.onClick.AddListener(OnVentureForthClicked);
        }
    }

    void OnVentureForthClicked()
    {
        string input1 = inputField1.text;
        string input2 = inputField2.text;

        if (string.IsNullOrEmpty(input1) || string.IsNullOrEmpty(input2))
        {
            NotifyPlayer("Please fill in all fields before proceeding.");
        }
        else
        {
            // Proceed with your actions
            Debug.Log("Venture Forth button clicked with valid inputs!");
            // Add your custom actions here
        }
    }

    void NotifyPlayer(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            // Optionally, add logic to hide the notification after a few seconds
            StartCoroutine(HideNotificationAfterDelay(3f)); // Hide after 3 seconds
        }
    }

    IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}
