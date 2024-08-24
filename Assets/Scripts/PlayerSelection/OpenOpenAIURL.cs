using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
    // URL to open
    public string url = "https://platform.openai.com/settings/profile?tab=api-keys";

    // Method to be called when the button is clicked
    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
