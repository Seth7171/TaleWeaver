using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text Luck_text;
    public static LuckBar Instance { get; private set; }

    private void Start()
    {
        // NO MORE SINGELTON STATIC INSTANCE WILL BE CHANGED BETWEEN 1 TO 10 PAGES!
        Instance = this;
        if (PlayerInGame.Instance != null)
        {
            SetLuck(PlayerInGame.Instance.currentLuck);
        }
    }

    public void SetLuck(int luck)
    {
        slider.value = luck;
        Luck_text.text = luck.ToString() + "/" + slider.maxValue.ToString();
    }

    public void SetMaxLuck(int luck)
    {
        slider.maxValue = luck;
        slider.value = luck;
        Luck_text.text = luck.ToString() + "/" + slider.maxValue.ToString();
    }



}
