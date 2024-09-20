// Filename: InputValidator.cs
// Author: Nitsan Maman & Ron Shahar
// Description: This static class validates input strings to ensure they contain only allowed characters.

using System.Text.RegularExpressions;

/// <summary>
/// This class provides functionality to validate input strings based on allowed character patterns.
/// </summary>
public static class InputValidator
{
    // Regular expression pattern for allowed characters (letters, numbers, and specific punctuation)
    private static readonly string AllowedCharactersPattern = @"^[a-zA-Z0-9!?., ]*$";

    /// <summary>
    /// Checks if the input string contains only allowed characters.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns>True if the input is valid; otherwise, false.</returns>
    public static bool IsValidInput(string input)
    {
        return Regex.IsMatch(input, AllowedCharactersPattern); // Validate input against the allowed characters pattern
    }
}
