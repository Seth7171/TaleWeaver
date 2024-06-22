using System.Text.RegularExpressions;

public static class InputValidator
{
    private static readonly string AllowedCharactersPattern = @"^[a-zA-Z0-9!?., ]*$";

    public static bool IsValidInput(string input)
    {
        return Regex.IsMatch(input, AllowedCharactersPattern);
    }
}
