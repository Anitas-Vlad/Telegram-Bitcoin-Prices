namespace TelegramBitcoinPrices.Interfaces;

public interface ITelegramBitcoinPricesNotifier
{
    Task Start(string input);
    Task Stop();

    Task Run();
}