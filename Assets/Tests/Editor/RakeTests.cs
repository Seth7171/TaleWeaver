// Located at Assets/Tests/Editor/LocationExtractorTests.cs

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RakeNamespace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

public class LocationExtractorTests
{
    private string stopWordsFilePath;
    private string originalStopWordsContent;

    [SetUp]
    public void Setup()
    {
        // Define the path to the stop words file
        stopWordsFilePath = Path.Combine(Application.dataPath, "Scripts/Environments/RAKE/SmartStoplist.txt");

        // Backup original stop words content if the file exists
        if (File.Exists(stopWordsFilePath))
        {
            originalStopWordsContent = File.ReadAllText(stopWordsFilePath);
        }

        // Create a temporary stop words file for testing
        var testStopWords = new HashSet<string>
        {
            "the", "and", "to", "in", "at", "they", "a", "an",
            "of", "on", "for", "with", "as", "is", "are", "was", "were", "but", "or"
        };
        File.WriteAllText(stopWordsFilePath, string.Join("\n", testStopWords));

        // Initialize the LocationExtractor singleton
        // Ensure that any previous instance is destroyed
        if (LocationExtractor.Instance != null)
        {
            // Use reflection to reset the singleton instance
            var instanceField = typeof(LocationExtractor).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
            if (instanceField != null)
            {
                instanceField.SetValue(null, null);
            }
        }

        // Access the Instance to trigger initialization
        var extractorInstance = LocationExtractor.Instance;
    }

    [TearDown]
    public void Teardown()
    {
        // Restore original stop words content if it was backed up
        if (originalStopWordsContent != null)
        {
            File.WriteAllText(stopWordsFilePath, originalStopWordsContent);
        }
        else
        {
            // Delete the temporary stop words file if it was created during the test
            if (File.Exists(stopWordsFilePath))
            {
                File.Delete(stopWordsFilePath);
            }
        }

        // Reset the LocationExtractor singleton
        var instanceFieldReset = typeof(LocationExtractor).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceFieldReset != null)
        {
            instanceFieldReset.SetValue(null, null);
        }

        // Optionally, clear KnownLocations.Locations if needed
        // KnownLocations.Locations.Clear();
        // However, since KnownLocations.Locations is static and likely populated once, consider if this is necessary
    }

    [Test]
    public void ExtractLocation_Returns_Correct_Location_When_Present()
    {
        // Arrange
        string text = "Adventurers gathered at the tavern. They prepared to explore the dungeon and the forest. Later, they rested in the library.";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        // Expected location indicators: "tavern", "dungeon", "forest", "library"
        // We expect "tavern" to have the highest score due to location indicators boosting
        string expectedLocation = "tavern";
        Assert.AreEqual(expectedLocation, extractedLocation, $"Expected location '{expectedLocation}', but got '{extractedLocation}'.");
    }

    [Test]
    public void ExtractLocation_Returns_None_When_No_Location_Present()
    {
        // Arrange
        string text = "The adventurers gathered and prepared to explore new territories.";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        Assert.AreEqual("None", extractedLocation, $"Expected 'None' when no location indicators are present, but got '{extractedLocation}'.");
    }

    [Test]
    public void ExtractLocation_Returns_First_Location_With_Highest_Score()
    {
        // Arrange
        string text = "Going to hell to the watch the fire beneath the volcano, then we will relax in the meadows";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        string expectedLocation = "hell";
        Assert.AreEqual(expectedLocation, extractedLocation, $"Expected location '{expectedLocation}', but got '{extractedLocation}'.");
    }

    [Test]
    public void ExtractLocation_Ignores_Stop_Words_Correctly()
    {
        // Arrange
        string text = "A adventurer went to the cave where is why do it.";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        string expectedLocation = "cave";
        Assert.AreEqual(expectedLocation, extractedLocation, $"Expected location '{expectedLocation}', but got '{extractedLocation}'.");
    }

    [Test]
    public void ExtractLocation_Returns_Location_From_Multiple_Indicators()
    {
        // Arrange
        string text = "The pirates attacked the swamp and then retreated to their hideout.";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        // Possible extracted locations: "pirates", "swamp", "hideout" (if mapped)
        var expectedLocations = new List<string> { "pirates", "swamp" };
        Assert.Contains(extractedLocation, expectedLocations, $"Extracted location '{extractedLocation}' is not among the expected locations.");
    }

    [Test]
    public void ExtractLocation_Handles_Punctuation_Correctly()
    {
        // Arrange
        string text = "Exploring the dunes of the desert, the adventurers faced many challenges! Did they survive? Only time will tell.";

        // Act
        string extractedLocation = LocationExtractor.Instance.ExtractLocation(text);

        // Assert
        string expectedLocation = "desert";
        Assert.AreEqual(expectedLocation, extractedLocation, $"Expected location '{expectedLocation}' after handling punctuation, but got '{extractedLocation}'.");
    }
}
