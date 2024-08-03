using echo17.EndlessBook;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class PageFlipper : MonoBehaviour
{
    public static PageFlipper Instance { get; private set; }
    public EndlessBook book;
    public List<GameObject> advObjects;
    private int currentAdvIndex = 0;


    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (OpenAIInterface.Instance != null)
                OpenAIInterface.Instance.OnIsEndedChanged -= OnIsEndedChanged;
            // This means this instance was the singleton and is now being destroyed
            Debug.Log("PageFlipper instance is being destroyed.");
            Instance = null;
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Disable all ADV objects initially
        foreach (var adv in advObjects)
        {
            adv.SetActive(false);
        }

        // Enable the first ADV object
        if (advObjects.Count > 0)
        {
            advObjects[0].SetActive(true);
        }

        OpenAIInterface.Instance.OnIsEndedChanged += OnIsEndedChanged;
    }

    private void OnIsEndedChanged(bool isEnded)
    {
        if (isEnded)
        {
            string mechanic = GameMechanicsManager.Instance.currentMechnism;
            EnableNextAdv(mechanic);
            // TO DO : finish the mini loading screen inside the book
            book.TurnToPage(book.CurrentLeftPageNumber + 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
        }
    }

    private void EnableNextAdv(string mechanic)
    {
        if (currentAdvIndex < advObjects.Count - 1)
        {
            currentAdvIndex++;
            advObjects[currentAdvIndex].SetActive(true);
            GameMechanicsManager.Instance.setMechanism(mechanic);
        }
    }
}
