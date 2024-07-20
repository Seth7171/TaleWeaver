using echo17.EndlessBook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EndlessBook book;
    public List<GameObject> advObjects;
    private int currentAdvIndex = 0;
    private OptionsMechanics optionsMechanics;
    private GameObject optionsMechanicsCanvas;
    int mechanisemType = 1;


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
        optionsMechanics = FindObjectOfType<OptionsMechanics>();

        chooseMechanism();
    }

    private void chooseMechanism()
    {
        if (mechanisemType == 1)
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
            EnableNextAdv();
            book.TurnToPage(book.CurrentLeftPageNumber + 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
        }
    }

    private void EnableNextAdv()
    {
        if (currentAdvIndex < advObjects.Count - 1)
        {
            currentAdvIndex++;
            advObjects[currentAdvIndex].SetActive(true);
            chooseMechanism();
        }
    }
}
