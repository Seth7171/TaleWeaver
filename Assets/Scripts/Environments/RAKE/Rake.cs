using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace RakeNamespace
{
    public class Rake
    {
        private readonly HashSet<string> _stopWords;
        private readonly HashSet<string> _locationIndicators; // Additional set for location hints

        public Rake(HashSet<string> stopWords)
        {
            _stopWords = stopWords ?? new HashSet<string>();
            // Initialize with typical location indicators, customize as necessary for your domain
            _locationIndicators = new HashSet<string> { "library", "office", "desert", "dungeon", "pirates",
                "village", "swamp", "asia", "tavern", "cave", "mountain", "space", "forest",
                "hell", "meadow", "city", "graveyard" };
        }

        public Dictionary<string, double> Run(string text)
        {
            var sentences = SplitSentences(text.ToLowerInvariant());
            var phrases = GenerateCandidateKeywords(sentences);
            var wordScores = CalculateWordScores(phrases);
            return GenerateCandidateKeywordScores(phrases, wordScores);
        }

        private List<string> GenerateCandidateKeywords(string[] sentences)
        {
            List<string> phrases = new List<string>();
            foreach (var sentence in sentences)
            {
                var words = sentence.Split(' ').Where(w => !_stopWords.Contains(w) && w.Length > 1).ToList();
                string phrase = "";
                foreach (var word in words)
                {
                    if (_stopWords.Contains(word))
                    {
                        if (!string.IsNullOrEmpty(phrase))
                        {
                            phrases.Add(phrase.Trim());
                            phrase = "";
                        }
                    }
                    else
                    {
                        phrase += word + " ";
                    }
                }
                if (!string.IsNullOrEmpty(phrase))
                    phrases.Add(phrase.Trim());
            }
            return phrases;
        }

        private Dictionary<string, double> CalculateWordScores(IEnumerable<string> phrases)
        {
            var wordFrequency = new Dictionary<string, double>();
            var wordDegree = new Dictionary<string, double>();

            foreach (var phrase in phrases)
            {
                var words = phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var wordsLength = words.Length;
                var wordListDegree = wordsLength - 1;

                foreach (var word in words)
                {
                    if (!wordFrequency.ContainsKey(word))
                        wordFrequency[word] = 0;

                    if (!wordDegree.ContainsKey(word))
                        wordDegree[word] = 0;

                    wordFrequency[word]++;
                    wordDegree[word] += wordListDegree; // Degree is sum of all degrees
                }
            }

            return wordFrequency.Keys.ToDictionary(word => word, word => (wordDegree[word] + wordFrequency[word]) / wordFrequency[word]);
        }

        private Dictionary<string, double> GenerateCandidateKeywordScores(List<string> phrases, Dictionary<string, double> wordScores)
        {
            var keywordScores = new Dictionary<string, double>();
            foreach (var phrase in phrases)
            {
                var words = phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                double score = words.Sum(word => wordScores.ContainsKey(word) ? wordScores[word] : 0);

                // Boost score for phrases containing location indicators
                if (words.Any(w => _locationIndicators.Contains(w)))
                    score *= 1.5; // Adjust multiplier as needed

                keywordScores[phrase] = score;
            }
            return keywordScores;
        }

        private static string[] SplitSentences(string text)
        {
            // Basic sentence delimiter based splitting
            return Regex.Split(text, @"[.!?;\n]");
        }
    }
}