// Filename: Book.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This file defines classes related to the book structure in a storytelling or gaming application, detailing each part of an encounter scenario including options and outcomes.

using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Represents a book which could be part of a game or any other application requiring a structured narrative.
/// </summary>
[Serializable]
public class Book
{
    public string Name;  // The name of the book.
    public string Description;  // A short description of the book's content.
    public List<Page> Pages;  // A list containing all the pages of the book.

    /// <summary>
    /// Constructor for creating a new book with a specified name and description.
    /// Initializes the Pages list.
    /// </summary>
    /// <param name="name">Name of the book.</param>
    /// <param name="description">Description of the book.</param>
    public Book(string name, string description)
    {
        Name = name;
        Description = description;
        Pages = new List<Page>();
    }
}

/// <summary>
/// Represents an option within an encounter, detailing possible outcomes and the mechanics of choice.
/// </summary>
[Serializable]
public class Option
{
    public string option;  // The text of the option.
    public string outcome;  // The resulting outcome if this option is chosen.
    public bool selectedAnswer;  // Indicates whether this option has been selected.
    public int diceResult;  // The result of a dice roll, if applicable to the option.

    /// <summary>
    /// Constructor for creating a new option with details about its outcome and selection state.
    /// </summary>
    /// <param name="option">The text of the option.</param>
    /// <param name="outcome">The outcome of choosing this option.</param>
    /// <param name="selectedAnswer">Initial selected state of the option (default is false).</param>
    /// <param name="diceResult">Dice result associated with the option (default is 0).</param>
    public Option(string option, string outcome, bool selectedAnswer = false, int diceResult = 0)
    {
        this.option = option;
        this.outcome = outcome;
        this.selectedAnswer = selectedAnswer;
        this.diceResult = diceResult;
    }
}

/// <summary>
/// Represents a page within the book, detailing the scenario, images, options, and other related details.
/// </summary>
[Serializable]
public class Page
{
    public string EncounterNum;  // Unique identifier for the encounter.
    public string EncounterName;  // The name of the encounter.
    public string EncounterIntroduction;  // Introductory text for the encounter.
    public string ImageGeneration;  // Descriptions or commands for generating associated imagery.
    public string EncounterDetails;  // Detailed description of what the encounter entails.
    public string EncounterMechanic;  // The primary mechanic used in the encounter.
    public string EncounterMechanicInfo;  // Additional information about the mechanic.
    public List<Option> EncounterOptions;  // A list of options available in this encounter.
    public string ImageUrl;  // URL to an image associated with the encounter.
    public List<int> EncounterStats;  // Statistical information related to the encounter.

    /// <summary>
    /// Default constructor for a page.
    /// </summary>
    public Page()
    {
    }

    /// <summary>
    /// Constructor for creating a fully detailed page with all encounter components.
    /// </summary>
    /// <param name="encounterNum">Encounter number.</param>
    /// <param name="encounterName">Name of the encounter.</param>
    /// <param name="encounterIntroduction">Introduction to the encounter.</param>
    /// <param name="imageGeneration">Instructions for generating associated imagery.</param>
    /// <param name="encounterDetails">Details about the encounter.</param>
    /// <param name="encounterMechanic">Main mechanic of the encounter.</param>
    /// <param name="encounterOptions">List of options for the encounter.</param>
    /// <param name="imageUrl">URL of an image for the encounter.</param>
    /// <param name="encounterStats">List of statistics for the encounter.</param>
    /// <param name="encounterMechanicInfo">Additional info about the encounter mechanic.</param>
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
