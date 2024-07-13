using UnityEngine;

public class SetResolution : MonoBehaviour
{
    void Start()
    {
        // Set the resolution to 1920x1080 (Full HD)
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
