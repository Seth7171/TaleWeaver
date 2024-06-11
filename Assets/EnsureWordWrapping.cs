using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnsureWordWrapping : MonoBehaviour
{
    public TMP_Text textComponent;
    public TMP_Text placeholderComponent;

    void Start()
    {
        if (textComponent != null)
        {
            textComponent.enableWordWrapping = true;
        }

        if (placeholderComponent != null)
        {
            placeholderComponent.enableWordWrapping = true;
        }
    }
}