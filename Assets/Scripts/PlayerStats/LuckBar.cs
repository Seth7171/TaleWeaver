using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LuckBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text Luck_text;


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
