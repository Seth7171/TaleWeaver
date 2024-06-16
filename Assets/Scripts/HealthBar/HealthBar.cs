using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text HP_text;


    public void SetHealth(int health)
    {
        slider.value = health;
        HP_text.text = health.ToString() + "/" + slider.maxValue.ToString();
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        HP_text.text = health.ToString() + "/" + slider.maxValue.ToString();
    }



}
