using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandBookController : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform book;
    public Boolean is_readMode;
    public Boolean is_flashLight_on;
    public Light flashLight; // Add a reference to the light

    public MonoBehaviour cameraController; // Add your camera controller script here
    public MonoBehaviour characterController; // Add your character controller script here

    private Vector3 hiddenPositionRightHand = new Vector3(0.304f, -0.352f, -2.561f); // Hidden near the body
    private Vector3 hiddenPositionLeftHand = new Vector3(-0.2f, -0.45f, -1.2f); // Hidden near the body
    private Vector3 hiddenPositionBook = new Vector3(0.33f, -0.2f, 0.95f); // Hidden near the body

    private Vector3 midRightHandPosition = new Vector3(0.43f, 0.047f, -0.861f);  // Mid position for the right hand
    private Vector3 midBookPosition = new Vector3(0.204f, -0.192f, 0.871f); // Mid position for the book

    private Vector3 readLeftHandPosition = new Vector3(-0.2f, 0.35f, 0.4f); // Read position for the left hand
    private Vector3 readRightHandPosition = new Vector3(0.43f, 0.047f, -0.861f); // Read position for the right hand
    private Vector3 readBookPosition = new Vector3(0.087f, -0.454f, 0.68f); // Read position for the book

    private Quaternion hiddenRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion midRightHandRotation = Quaternion.Euler(0, -90, 0); // Adjust to match natural hand rotation
    private Quaternion midRightLowerArmRotation = Quaternion.Euler(12.382f, 342.163f, 364.797f); // Mid position for the right hand
    private Quaternion midLeftLowerArmRotation = Quaternion.Euler(-25.327f, 24.188f, -13.94f); // Read position for the right hand
    private Quaternion midBookRotation = Quaternion.Euler(11.786f, -23.299f, 143.51f); // Adjust as needed
    private Quaternion readLeftHandRotation = Quaternion.Euler(0, 180, -150); // Adjust to match natural hand rotation
    private Quaternion readLeftLowerArmRotation = Quaternion.Euler(13.834f, -31.093f, 30.042f); // Read position for the right hand
    private Quaternion readRightHandRotation = Quaternion.Euler(0, 180, 150); // Adjust to match natural hand rotation
    private Quaternion readRightLowerArmRotation = Quaternion.Euler(22.192f, 331.241f, 375.964f); // Read position for the right hand
    private Quaternion readBookRotation = Quaternion.Euler(22.649f, -88.853f, 91.079f); // Adjust as needed

    private Vector3 midBookScale = new Vector3(0.5016249f, 0.5016249f, 0.5016249f);
    private Vector3 readBookScale = new Vector3(0.7127827f, 0.7127827f, 0.7127827f);

    private int currentView = 0; // 0 = hidden, 1 = mid, 2 = read

    private Coroutine transitionCoroutine;

    public TextMeshProUGUI encounterOptions1_copy;
    public TextMeshProUGUI encounterOptions2_copy;
    public TextMeshProUGUI encounterOptions3_copy;

    void Start()
    {
        //Debug.Log("Start: Setting Hidden Position");
        SetHiddenPosition();
        is_readMode = false;
        is_flashLight_on = false;

        // Initially hide the encounter options
        HideEncounterOptions();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            currentView++;
            if (currentView > 2) currentView = 2;
        }
        else if (scrollInput < 0f)
        {
            currentView--;
            if (currentView < 0) currentView = 0;
        }

        //Debug.Log($"Update: Current View - {currentView}");

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        switch (currentView)
        {
            case 0:
                HideEncounterOptions();
                EnableControls();
                transitionCoroutine = StartCoroutine(TransitionToHidden());
                Cursor.visible = false;
                break;
            case 1:
                HideEncounterOptions();
                EnableControls();
                transitionCoroutine = StartCoroutine(TransitionToMid());
                Cursor.visible = false;
                break;
            case 2:
                transitionCoroutine = StartCoroutine(TransitionToRead());
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
                DisableControls();
                StartCoroutine(ShowEncounterOptionsWithDelay());
                break;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f pressed");
            if (is_readMode)
            {
                Debug.Log("and reading is on");
                if (is_flashLight_on)
                {
                    Debug.Log("and flash ligh turn off");
                    flashLight.enabled = false;
                }

                else
                {
                    Debug.Log("and flash ligh turn on");
                    flashLight.enabled = true;
                }     
            }

        }
    }

    void SetHiddenPosition()
    {
        is_readMode = false;
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform rightInnerHand = rightHand.Find("hand_right");

        if (leftInnerHand != null)
        {
            leftInnerHand.localPosition = hiddenPositionLeftHand;
            leftInnerHand.localRotation = hiddenRotation;
            leftInnerHand.gameObject.SetActive(false);
        }

        if (rightInnerHand != null)
        {
            rightHand.localPosition = hiddenPositionRightHand;
            rightInnerHand.localRotation = hiddenRotation;
            rightInnerHand.gameObject.SetActive(false);
        }

        if (book != null)
        {
            book.localPosition = hiddenPositionBook;
            book.localRotation = hiddenRotation;
            book.gameObject.SetActive(false);
        }

        EnableControls();
    }

    public void DisableControls()
    {
        if (cameraController != null)
        {
            //Debug.Log("Disabling Camera Controller");
            cameraController.enabled = false;
        }

        if (characterController != null)
        {
            //Debug.Log("Disabling Character Controller");
            characterController.enabled = false;
        }
    }

    public void EnableControls()
    {
        if (cameraController != null)
        {
            //Debug.Log("Enabling Camera Controller");
            cameraController.enabled = true;
        }

        if (characterController != null)
        {
            //Debug.Log("Enabling Character Controller");
            characterController.enabled = true;
        }
    }

    IEnumerator TransitionToHidden()
    {
        //Debug.Log("Transition to Hidden");
        HideEncounterOptions();
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true);
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true);
        }

        if (book != null)
        {
            book.gameObject.SetActive(true);
        }

        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.5f; // Transition duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rightHand.localPosition = Vector3.Lerp(startRightHandPosition, hiddenPositionRightHand, t);
            rightInnerHand.localRotation = Quaternion.Lerp(startRightHandRotation, hiddenRotation, t);
            rightLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startRightLowerArmRotation), hiddenRotation, t);
            leftInnerHand.localPosition = Vector3.Lerp(startLeftHandPosition, hiddenPositionLeftHand, t);
            leftInnerHand.localRotation = Quaternion.Lerp(startLeftHandRotation, hiddenRotation, t);
            leftLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startLeftLowerArmRotation), midLeftLowerArmRotation, t);
            book.localPosition = Vector3.Lerp(startBookPosition, hiddenPositionBook, t);
            book.localRotation = Quaternion.Lerp(startBookRotation, hiddenRotation, t);
            book.localScale = Vector3.Lerp(startBookScale, Vector3.zero, t);

            yield return null;
        }

        SetHiddenPosition();
    }

    IEnumerator TransitionToMid()
    {
        is_readMode = false;
        HideEncounterOptions();
        //Debug.Log("Transition to Mid");
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true);
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true);
        }

        if (book != null)
        {
            book.gameObject.SetActive(true);
        }

        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.2f; // Transition duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rightHand.localPosition = Vector3.Lerp(startRightHandPosition, midRightHandPosition, t);
            rightInnerHand.localRotation = Quaternion.Lerp(startRightHandRotation, midRightHandRotation, t);
            rightLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startRightLowerArmRotation), midRightLowerArmRotation, t);
            leftInnerHand.localPosition = Vector3.Lerp(startLeftHandPosition, hiddenPositionLeftHand, t);
            leftInnerHand.localRotation = Quaternion.Lerp(startLeftHandRotation, hiddenRotation, t);
            leftLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startLeftLowerArmRotation), midLeftLowerArmRotation, t);
            book.localPosition = Vector3.Lerp(startBookPosition, midBookPosition, t);
            book.localRotation = Quaternion.Lerp(startBookRotation, midBookRotation, t);
            book.localScale = Vector3.Lerp(startBookScale, midBookScale, t);

            yield return null;
        }

        rightHand.localPosition = midRightHandPosition;
        rightInnerHand.localRotation = midRightHandRotation;
        rightLowerArm.localRotation = midRightLowerArmRotation;
        leftInnerHand.localPosition = hiddenPositionLeftHand;
        leftInnerHand.localRotation = hiddenRotation;
        leftLowerArm.localRotation = midLeftLowerArmRotation;
        leftInnerHand.gameObject.SetActive(false);
        book.localPosition = midBookPosition;
        book.localRotation = midBookRotation;
        book.localScale = midBookScale;

        EnableControls();
    }

    IEnumerator TransitionToRead()
    {
        is_readMode = true;

        //Debug.Log("Transition to Read");
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true);
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true);
        }

        if (book != null)
        {
            book.gameObject.SetActive(true);
        }

        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.2f; // Transition duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rightHand.localPosition = Vector3.Lerp(startRightHandPosition, readRightHandPosition, t);
            rightInnerHand.localRotation = Quaternion.Lerp(startRightHandRotation, readRightHandRotation, t);
            rightLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startRightLowerArmRotation), readRightLowerArmRotation, t);
            leftInnerHand.localPosition = Vector3.Lerp(startLeftHandPosition, readLeftHandPosition, t);
            leftInnerHand.localRotation = Quaternion.Lerp(startLeftHandRotation, readLeftHandRotation, t);
            leftLowerArm.localRotation = Quaternion.Lerp(Quaternion.Euler(startLeftLowerArmRotation), readLeftLowerArmRotation, t);
            book.localPosition = Vector3.Lerp(startBookPosition, readBookPosition, t);
            book.localRotation = Quaternion.Lerp(startBookRotation, readBookRotation, t);
            book.localScale = Vector3.Lerp(startBookScale, readBookScale, t);

            yield return null;
        }

        rightHand.localPosition = readRightHandPosition;
        rightInnerHand.localRotation = readRightHandRotation;
        rightLowerArm.localRotation = readRightLowerArmRotation;
        leftInnerHand.localPosition = readLeftHandPosition;
        leftInnerHand.localRotation = readLeftHandRotation;
        leftLowerArm.localRotation = readLeftLowerArmRotation;
        book.localPosition = readBookPosition;
        book.localRotation = readBookRotation;
        book.localScale = readBookScale;

        DisableControls();
    }
    void HideEncounterOptions()
    {
        encounterOptions1_copy.gameObject.SetActive(false);
        encounterOptions2_copy.gameObject.SetActive(false);
        encounterOptions3_copy.gameObject.SetActive(false);
    }

    IEnumerator ShowEncounterOptionsWithDelay()
    {
        yield return new WaitForSeconds(1f); // Delay before showing the encounter options
        if (is_readMode == true)
        {
            ShowEncounterOptions();
        }
    }

    void ShowEncounterOptions()
    {
        encounterOptions1_copy.gameObject.SetActive(true);
        encounterOptions2_copy.gameObject.SetActive(true);
        encounterOptions3_copy.gameObject.SetActive(true);
    }
}
