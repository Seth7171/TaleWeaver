using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

[Serializable]
public class Book
{
    public string Name;
    public string Description;
    public List<Page> Pages;

    public Book(string name, string description)
    {
        Name = name;
        Description = description;
        Pages = new List<Page>();
    }
}

[Serializable]
public class Option
{
    public string option;
    public string outcome;
    public bool selectedAnswer;
    public int diceResult;

    public Option(string option, string outcome, bool selectedAnswer = false, int diceResult = 0)
    {
        this.option = option;
        this.outcome = outcome;
        this.selectedAnswer = selectedAnswer;
        this.diceResult = diceResult;
    }
}


[Serializable]
public class Page
{
    public string EncounterNum;
    public string EncounterName;
    public string EncounterIntroduction;
    public string ImageGeneration;
    public string EncounterDetails;
    public string EncounterMechanic; // the mechanic 
    public string EncounterMechanicInfo;
    public List<Option> EncounterOptions;
    public string ImageUrl;
    public List<int> EncounterStats;

    public Page()
    {
    }

    public Page(string encounterNum, string encounterName, string encounterIntroduction, string imageGeneration, string encounterDetails, string encounterMechanic, List<Option> encounterOptions, string imageUrl, List<int> encounterStats, string encounterMechanicInfo = "")
    {
        EncounterNum = encounterNum;
        EncounterName = encounterName;
        EncounterIntroduction = encounterIntroduction;
        ImageGeneration = imageGeneration;
        EncounterDetails = encounterDetails;
        EncounterMechanic = encounterMechanic;
        EncounterMechanicInfo = encounterMechanicInfo;
        EncounterOptions = encounterOptions;
        ImageUrl = imageUrl;
        EncounterStats = encounterStats;
    }
}

