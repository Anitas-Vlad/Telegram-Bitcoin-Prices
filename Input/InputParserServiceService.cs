﻿using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices.Input;

public class InputParserServiceServiceService : IInputParserService
{
    public BtcRange ParseBtcRange(string input)
    {
        InputValidator.ValidateStartCommand(input);

        var parts = input.Split(' ');
        var lowBtcPrice = decimal.Parse(parts[1]);
        var highBtcPrice = decimal.Parse(parts[2]);

        if (lowBtcPrice >= highBtcPrice)
            throw new ArgumentException("The first number must be less than the second number.");

        return new BtcRange
        {
            LowBtcPrice = lowBtcPrice,
            HighBtcPrice = highBtcPrice
        };
    }
}