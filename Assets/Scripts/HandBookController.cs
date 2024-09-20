// Filename: HandBookController.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This class manages the interactions and animations for the handbook in the game, including reading mode and flashlight control.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the interactions and animations for the handbook in the game.
/// </summary>
public class HandBookController : MonoBehaviour
{
    public Transform leftHand; // Reference to the left hand's transform
    public Transform rightHand; // Reference to the right hand's transform
    public Transform book; // Reference to the book's transform
    public bool is_readMode; // Indicates if the book is in read mode
    public bool is_flashLight_on; // Indicates if the flashlight is on
    public bool is_scroll_lock = false; // Prevents scrolling when true

    public Light flashLight; // reference to the light

    public MonoBehaviour cameraController; // camera controller script 
    public MonoBehaviour characterController; //  character controller

    // Various positions and rotations for hands and the book
    private Vector3 hiddenPositionRightHand = new Vector3(0.2318f, 0.0434f, -1.0674f); // Hidden near the body
    private Vector3 hiddenPositionLeftHand = new Vector3(-0.2f, -0.45f, -1.2f); // Hidden near the body
    private Vector3 hiddenPositionBook = new Vector3(0.33f, -0.2f, 0.9f); // Hidden near the body
    private Vector3 hiddenScaleBook = new Vector3(0.2f, 0.2f, 0.2f); // Hidden near the body

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

    private int currentView = 1; // 0 = hidden, 1 = mid, 2 = read
    private int targetView = 1; // to track the desired view state

    private Coroutine transitionCoroutine;

    public Transform[] bookmarks; // Assume bookmarks are stored here
    private Coroutine bookmarkCoroutine;
    private bool buttonOnBookmarksClicked = false;
    private bool _isBookMarkShown = false;

    /// <summary>
    /// Initializes the HandBookController.
    /// </summary>
    void Start()
    {
        //Debug.Log("Start: Setting Hidden Position");
        //SetHiddenPosition();
        //TransitionToMid();
        is_readMode = false;
        is_flashLight_on = false;


        // Initially hide the encounter options
        HideEncounterOptions();


    }

    private void Awake()
    {
        // Initially hide the encounter options
        HideEncounterOptions();
    }

    /// <summary>
    /// Updates the state of the handbook based on user input.
    /// </summary>
    void Update()
    {
        // Handle scrolling input
        if (!is_scroll_lock)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0f)
            {
                if (targetView == 0)
                {
                    targetView = 1;
                }
                else if (targetView == 1)
                {
                    targetView = 2;
                }
            }
            else if (scrollInput < 0f)
            {
                if (targetView == 2)
                {
                    targetView = 1;
                }
                else if (targetView == 1)
                {
                    targetView = 1; //WE DISABLED THE BOOK HIDE!!! WHY? CUZ!
                }
            }
        }

        //Debug.Log($"Update: Target View - {targetView}");
        // Start the transition coroutine if it's not running
        if (transitionCoroutine == null)
        {
            transitionCoroutine = StartCoroutine(TransitionToView(targetView));
        }
        // Toggle flashlight on/off
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

    /// <summary>
    /// Transitions to the specified view state.
    /// </summary>
    /// <param name="view">The target view state (0 = hidden, 1 = mid, 2 = read).</param>
    public IEnumerator TransitionToView(int view)
    {
        while (currentView != view)
        {
            if (currentView < view)
            {
                currentView++;
            }
            else if (currentView > view)
            {
                currentView--;
            }

            switch (currentView)
            {
                //hidden case
                case 0:
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    HideEncounterOptions();
                    EnableControls();
                    yield return StartCoroutine(TransitionToHidden());
                    break;
                //mid case
                case 1:
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    HideEncounterOptions();
                    EnableControls();
                    yield return StartCoroutine(TransitionToMid());
                    break;
                //read case
                case 2:
                    yield return StartCoroutine(TransitionToRead());
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None; // Free the mouse cursor
                    DisableControls();
                    StartCoroutine(ShowEncounterOptionsWithDelay());
                    break;
            }
        }

        transitionCoroutine = null;
    }

    /// <summary>
    /// Sets the hands and book to the hidden position.
    /// </summary>
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
            book.localScale = hiddenScaleBook;
            book.localRotation = midBookRotation;
            book.gameObject.SetActive(false);
        }

        EnableControls();
    }

    /// <summary>
    /// Disables player controls during interactions.
    /// </summary>
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

    /// <summary>
    /// Enables player controls after interactions.
    /// </summary>
    public void EnableControls()
    {
        // if in book read mode skip (if the function called from pausemenu)
        if (currentView != 2)
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
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// Transitions to the hidden position of the hands and the book.
    /// </summary>
    /// <returns>A coroutine that smoothly animates the transition.</returns>
    IEnumerator TransitionToHidden()
    {
        //Debug.Log("Transition to Hidden");
        HideEncounterOptions(); // Hide any active encounter options
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true); // Activate right hand
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true); // Activate left hand
        }

        if (book != null)
        {
            book.gameObject.SetActive(true); // Activate book
        }

        // Store initial positions and rotations
        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.4f; // Transition duration
        float elapsed = 0f;

        // Smoothly animate to the hidden position
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
            //book.localRotation = Quaternion.Lerp(startBookRotation, hiddenRotation, t);
            book.localScale = Vector3.Lerp(startBookScale, hiddenScaleBook, t);
             
            yield return null; // Wait for the next frame
        }

        SetHiddenPosition(); // Finalize the hidden position
    }

    /// <summary>
    /// Transitions to the mid position of the hands and the book.
    /// </summary>
    /// <returns>A coroutine that smoothly animates the transition.</returns>
    IEnumerator TransitionToMid()
    {
        is_readMode = false; // Ensure not in read mode
        HideEncounterOptions(); // Hide any active encounter options

        //Debug.Log("Transition to Mid");
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true); // Activate right hand
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true); // Activate left hand
        }

        if (book != null)
        {
            book.gameObject.SetActive(true); // Activate book
        }

        // Store initial positions and rotations
        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.4f; // Transition duration
        float elapsed = 0f;

        // Smoothly animate to the mid position
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

        // Set final positions
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

        EnableControls(); // Enable controls after transition
    }

    /// <summary>
    /// Transitions to the read position of the hands and the book.
    /// </summary>
    /// <returns>A coroutine that smoothly animates the transition.</returns>
    IEnumerator TransitionToRead()
    {
        is_readMode = true; // Set read mode to true

        //Debug.Log("Transition to Read");
        Transform rightInnerHand = rightHand.Find("hand_right");
        Transform rightLowerArm = rightHand.Find("upperarm_r/lowerarm1_r");
        Transform leftInnerHand = leftHand.Find("hand_left");
        Transform leftLowerArm = leftHand.Find("upperarm_l/lowerarm1_l");

        if (rightInnerHand != null)
        {
            rightInnerHand.gameObject.SetActive(true); // Activate right hand
        }

        if (leftInnerHand != null)
        {
            leftInnerHand.gameObject.SetActive(true); // Activate left hand
        }

        if (book != null)
        {
            book.gameObject.SetActive(true); // Activate book
        }

        // Store initial positions and rotations
        Vector3 startRightHandPosition = rightHand.localPosition;
        Quaternion startRightHandRotation = rightInnerHand.localRotation;
        Vector3 startRightLowerArmRotation = rightLowerArm.localRotation.eulerAngles;
        Vector3 startLeftHandPosition = leftInnerHand.localPosition;
        Quaternion startLeftHandRotation = leftInnerHand.localRotation;
        Vector3 startLeftLowerArmRotation = leftLowerArm.localRotation.eulerAngles;
        Vector3 startBookPosition = book.localPosition;
        Quaternion startBookRotation = book.localRotation;
        Vector3 startBookScale = book.localScale;

        float duration = 0.4f; // Transition duration
        float elapsed = 0f;

        // Smoothly animate to the read position
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

            yield return null; // Wait for the next frame
        }

        // Set final positions
        rightHand.localPosition = readRightHandPosition;
        rightInnerHand.localRotation = readRightHandRotation;
        rightLowerArm.localRotation = readRightLowerArmRotation;
        leftInnerHand.localPosition = readLeftHandPosition;
        leftInnerHand.localRotation = readLeftHandRotation;
        leftLowerArm.localRotation = readLeftLowerArmRotation;
        book.localPosition = readBookPosition;
        book.localRotation = readBookRotation;
        book.localScale = readBookScale;

        DisableControls(); // Disable controls during reading
    }

    /// <summary>
    /// Hides encounter options from the UI.
    /// </summary>
    public void HideEncounterOptions()
    {
        if (BookLoader.Instance != null)
        {
            if (!BookLoader.Instance.isLoading)
            {
                if (BookLoader.Instance.currentUI != null)
                    BookLoader.Instance.currentUI.SetActive(false); // Hide current UI
            }
            if (!BookLoader.Instance.isActionMade)
            {
                if (BookLoader.Instance.DiceRollerButton != null)
                    BookLoader.Instance.DiceRollerButton.SetActive(false); // Hide dice roller button
            }
        }
    }

    /// <summary>
    /// Shows encounter options after a delay.
    /// </summary>
    /// <returns>A coroutine that waits before showing the options.</returns>
    IEnumerator ShowEncounterOptionsWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay before showing the encounter options
        if (is_readMode == true)
        {
            ShowEncounterOptions(); // Show options if in read mode
        }
    }

    /// <summary>
    /// Displays encounter options in the UI.
    /// </summary>
    void ShowEncounterOptions()
    {
        if (BookLoader.Instance != null)
        {
            if (!BookLoader.Instance.isLoading)
            {
                if (BookLoader.Instance.currentUI != null)
                    BookLoader.Instance.currentUI.SetActive(true); // Show current UI
            }

            if (!BookLoader.Instance.isActionMade)
            {
                if (BookLoader.Instance.DiceRollerButton != null)
                    BookLoader.Instance.DiceRollerButton.SetActive(true); // Show dice roller button
            }
        }
    }

    /// <summary>
    /// Handles bookmark click events.
    /// </summary>
    /// <param name="bookmark">The bookmark that was clicked.</param>
    public void OnBookmarkClicked(Transform bookmark)
    {
        // If a bookmark animation is already playing, stop it
        /*        if (bookmarkCoroutine != null)
                {
                    StopCoroutine(bookmarkCoroutine);
                }*/
        if (bookmark.localPosition == new Vector3(0.011f, 0.1971f, -0.033f))
        {
            bookmarkCoroutine = StartCoroutine(MoveBookmarkIn(bookmark));
        }
        else
        {
            // Start the animation coroutine for the clicked bookmark
            bookmarkCoroutine = StartCoroutine(MoveBookmarkOut(bookmark));
        }
    }


    /// <summary>
    /// Moves the bookmark out of view.
    /// </summary>
    /// <param name="bookmark">The bookmark to move.</param>
    /// <returns>A coroutine that animates the movement.</returns>
    private IEnumerator MoveBookmarkOut(Transform bookmark)
    {
        Vector3 startPosition = bookmark.localPosition;
        Vector3 midPosition = startPosition + new Vector3(0.1127f, -0.0056f, -0.0639f); // Move out from the book
        Vector3 outPosition = new Vector3(0.011f, 0.1971f, -0.033f); // Move to a position in front of the book

        Quaternion startRotation = bookmark.localRotation;
        Quaternion midRotation = Quaternion.Euler(0, 60, 0); // Rotate slightly outward
        Quaternion outRotation = Quaternion.Euler(0, 0, 0); // Maintain rotation

        float duration = 0.5f; // Duration of each animation step
        float elapsed = 0f;

        // Move out animation (first phase)
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);

            bookmark.localPosition = Vector3.Lerp(startPosition, midPosition, t);
            bookmark.localRotation = Quaternion.Lerp(startRotation, midRotation, t);

            yield return null;
        }

        // Reset elapsed for second phase
        elapsed = 0f;

        // Move out animation (second phase)
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);

            bookmark.localPosition = Vector3.Lerp(midPosition, outPosition, t);
            bookmark.localRotation = Quaternion.Lerp(midRotation, outRotation, t);

            yield return null;
        }

        // Ensure the final position is set correctly
        bookmark.localPosition = outPosition;
        bookmark.localRotation = outRotation;

        _isBookMarkShown = false;
    }


    /// <summary>
    /// Moves the bookmark into view.
    /// </summary>
    /// <param name="bookmark">The bookmark to move.</param>
    /// <returns>A coroutine that animates the movement.</returns>
    private IEnumerator MoveBookmarkIn(Transform bookmark)
    {
        _isBookMarkShown = true;

        Vector3 outPosition = bookmark.localPosition;
        Vector3 midPosition = outPosition + new Vector3(0, 0, 0.1f); // Move closer to the book
        Vector3 inPosition = new Vector3(0.247f, 0.025f, 0.13f); // Default position

        if (bookmark.name.Contains("2"))
        {
            inPosition = new Vector3(0.247f, 0.025f, 0.04f);
        }
        if (bookmark.name.Contains("3"))
        {
            inPosition = new Vector3(0.247f, 0.025f, -0.05f);
        }
        if (bookmark.name.Contains("4"))
        {
            inPosition = new Vector3(0.247f, 0.025f, -0.14f);
        }

        Quaternion outRotation = bookmark.localRotation;
        Quaternion midRotation = Quaternion.Euler(0, 0, 0); // Slight adjustment when moving in
        Quaternion inRotation = Quaternion.Euler(0, 0, 0); // Default rotation

        if (buttonOnBookmarksClicked)
        {
            inPosition = new Vector3(0f, 0f, 0f); // Special case handling
            buttonOnBookmarksClicked = false;
        }

        float duration = 0.5f; // Duration of each animation step
        float elapsed = 0f;

        // Move in animation (first phase)
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);

            bookmark.localPosition = Vector3.Lerp(outPosition, midPosition, t);
            bookmark.localRotation = Quaternion.Lerp(outRotation, midRotation, t);

            yield return null;
        }

        // Reset elapsed for second phase
        elapsed = 0f;

        // Move in animation (second phase)
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);

            bookmark.localPosition = Vector3.Lerp(midPosition, inPosition, t);
            bookmark.localRotation = Quaternion.Lerp(midRotation, inRotation, t);

            yield return null;
        }

        // Ensure the final position is set correctly
        bookmark.localPosition = inPosition;
        bookmark.localRotation = inRotation;
    }

    /// <summary>
    /// Handles bookmark button clicks.
    /// </summary>
    /// <param name="bookmark">The clicked bookmark.</param>
    public void ButtonOnBookmarksClicked(Transform bookmark)
    {
        if (!_isBookMarkShown) // If the bookmark is not shown
        {
            buttonOnBookmarksClicked = true; // Set the flag
            StartCoroutine(MoveBookmarkIn(bookmark)); // Move the bookmark in
        }
    }
}