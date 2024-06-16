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
    public int MaxWords;

    public Option(string option, int maxWords)
    {
        this.option = option;
        MaxWords = maxWords;
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
    public string EncounterAction;
    public List<Option> EncounterOptions;
    public string ImageUrl;

    public Page(string encounterNum, string encounterName, string encounterIntroduction, string imageGeneration, string encounterDetails, string encounterAction, List<Option> encounterOptions, string imageUrl)
    {
        EncounterNum = encounterNum;
        EncounterName = encounterName;
        EncounterIntroduction = encounterIntroduction;
        ImageGeneration = imageGeneration;
        EncounterDetails = encounterDetails;
        EncounterAction = encounterAction;
        EncounterOptions = encounterOptions;
        ImageUrl = imageUrl;
    }
}
