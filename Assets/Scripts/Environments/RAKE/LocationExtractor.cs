using System.Collections.Generic;
using UnityEngine;
using RakeNamespace;
using System.IO;

public class LocationExtractor
{
    private static LocationExtractor _instance;
    public static LocationExtractor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocationExtractor();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private HashSet<string> _stopWords;
    private Rake _rake;

    private LocationExtractor() { }

    public void Initialize()
    {
        string filePath = Path.Combine(Application.dataPath, "Scripts/Environments/RAKE/SmartStoplist.txt");
        if (!File.Exists(filePath))
        {
            Debug.LogError("Stop words file not found at: " + filePath);
            return;
        }

        string fileContent = File.ReadAllText(filePath);
        _stopWords = new HashSet<string>(fileContent.Split('\n'));
        _rake = new Rake(_stopWords);
    }

    public string ExtractLocation(string text)
    {
        Dictionary<string, double> keywords = _rake.Run(text.ToLowerInvariant());
        foreach (var keyword in keywords.Keys)
        {
            string lowerKeyword = keyword.ToLowerInvariant();
            // Split the keyword into individual words
            string[] words = lowerKeyword.Split(' ');
            foreach (string word in words)
                if (KnownLocations.Locations.ContainsKey(word))
                {
                    return KnownLocations.Locations.GetValueOrDefault(word);
                }
        }

        return "None";
    }
}