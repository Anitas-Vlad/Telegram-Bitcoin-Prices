using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices.Input;

public class InputParser : IInputParser
{
    public BtcRange ParseBtcRange(string input)
    {
        InputValidator.Validate(input);

        var parts = input.Split(' ');
        var lowBtcPrice= decimal.Parse(parts[1]);
        var highBtcPrice= decimal.Parse(parts[2]);

        if (lowBtcPrice >= highBtcPrice)
            throw new ArgumentException("The first number must be less than the second number.");

        return new BtcRange()
        {
            LowBtcPrice= lowBtcPrice,
            HighBtcPrice = highBtcPrice
        };
    }
}