using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clear : MonoBehaviour
{
    public float displayDuration = 3f;
    public TMP_Text feedbackText;

    public void ClearFeedbackFromUnity()
    {
        StartCoroutine(ClearFeedbackText());
    }

    private IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackText.text = "";
    }
}
