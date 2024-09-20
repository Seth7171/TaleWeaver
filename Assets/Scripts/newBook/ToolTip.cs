// Filename: ToolTip.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This script manages tooltips that display information when the user hovers over UI elements.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class implements tooltip functionality for UI elements, displaying messages on pointer hover.
/// </summary>
public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Called when the pointer enters the UI element. Displays the tooltip with a message.
    /// </summary>
    /// <param name="eventData">The event data containing information about the pointer event.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        string message = "Office,\nDesert,\nDungeon,\nPirates,\nVillage,\nSwamp,\nAsia,\nTavern,\nCave,\nMountain,\nSpace,\nForest,\nHell,\nMeadow,\nCity,\nGraveyard.\n\nElse you will be playing in the library!";
        ToolTipManager.instance.SetAndShowToolTip(message);

    }

    /// <summary>
    /// Called when the pointer exits the UI element. Hides the tooltip.
    /// </summary>
    /// <param name="eventData">The event data containing information about the pointer event.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.instance.HideToolTip();
    }
}
