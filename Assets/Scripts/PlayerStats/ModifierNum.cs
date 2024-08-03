using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifierNum : MonoBehaviour
{
    //public Slider slider;
    public TMP_Text Check_Modifier_text;
    private Color lessBrightGreen = new Color(0.0f, 0.5f, 0.0f);


    public void SetCheckModifier(int modifier)
    {
        if (modifier > 0)
        {
            Check_Modifier_text.color = Color.red;
            Check_Modifier_text.text = "+" + modifier.ToString();
        }
        else if (modifier < 0)
        {
            Check_Modifier_text.color = lessBrightGreen;
            Check_Modifier_text.text = modifier.ToString();
        }
        else
        {
            Check_Modifier_text.color = Color.black;
            Check_Modifier_text.text = modifier.ToString();
        }
    }


}
