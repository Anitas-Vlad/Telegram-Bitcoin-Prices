using System.Text.RegularExpressions;

namespace TelegramBitcoinPrices.Input;

public static class InputValidator
{
    public static void ValidateStartCommand(string input)
    {
        var parts = input.Split(' ');

        // Check for valid format
        if (parts.Length != 3 || parts[0].ToLower() != "start")
            throw new ArgumentException("Invalid input format. Expected format: 'start <lowNumber> <highNumber>'.");

        if (!decimal.TryParse(parts[1], out _))
            throw new ArgumentException("The first number must be a valid decimal.");

        if (!decimal.TryParse(parts[2], out _))
            throw new ArgumentException("The second number must be a valid decimal.");
    }

    public static decimal ValidateAndExtractNumber(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return 0;

        var match = Regex.Match(message.Trim(), @"^buy\s+(\d+)$", RegexOptions.IgnoreCase);
        if (match.Success && decimal.TryParse(match.Groups[1].Value, out var number)) return number;

        return 0; // Return null if the format is incorrect
    }
}