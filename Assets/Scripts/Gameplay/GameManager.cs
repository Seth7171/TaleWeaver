using echo17.EndlessBook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public EndlessBook book;
    public List<GameObject> advObjects;
    private int currentAdvIndex = 0;
    private CreateButtonsInBook optionsMechanics;
    private GameObject optionsMechanicsCanvas;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        //OpenAIInterface.Instance.OnIsEndedChanged += OnIsEndedChanged;

        // Find the instances of all Mechanics in the scene
        optionsMechanics = FindObjectOfType<CreateButtonsInBook>();

        //seteMechanism();
    }

    public void buttonsInit()
    {
        optionsMechanics = FindObjectOfType<CreateButtonsInBook>();
    }

    public void setMechanism(string mechnism)
    {
        if (mechnism.Contains("options"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }

        if (mechnism.Contains("combat"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }

        if (mechnism.Contains("luck"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }

        if (mechnism.Contains("riddle"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }

        if (mechnism.Contains("roll"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }

        if (mechnism.Contains("check"))
        {
            if (optionsMechanics == null)
            {
                Debug.LogError("OptionsMechanics not found in the scene");
                return;
            }
            else
            {
                optionsMechanics.initialize();
            }
        }
    }

    private void OnDestroy()
    {
        OpenAIInterface.Instance.OnIsEndedChanged -= OnIsEndedChanged;
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
            setMechanism(mechanic);
        }
    }
}
