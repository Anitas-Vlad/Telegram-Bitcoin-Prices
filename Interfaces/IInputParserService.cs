using TelegramBitcoinPrices.Input;

namespace TelegramBitcoinPrices.Interfaces;

public interface IInputParserService
{
    BtcRange ParseBtcRange(string input);
}