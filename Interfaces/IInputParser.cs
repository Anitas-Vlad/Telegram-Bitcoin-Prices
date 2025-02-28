using TelegramBitcoinPrices.Input;

namespace TelegramBitcoinPrices.Interfaces;

public interface IInputParser
{
    BtcRange ParseBtcRange(string input);
}