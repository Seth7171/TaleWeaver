using System;
using System.Collections.Generic;

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
    public bool isCorrectAnswer;

    public Option(string option, string outcome, bool isCorrectAnswer = false)
    {
        this.option = option;
        this.outcome = outcome;
        this.isCorrectAnswer = isCorrectAnswer;
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

    public Page(string encounterNum, string encounterName, string encounterIntroduction, string imageGeneration, string encounterDetails, string encounterMechanic, List<Option> encounterOptions, string imageUrl, string encounterMechanicInfo = "")
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
    }
}

