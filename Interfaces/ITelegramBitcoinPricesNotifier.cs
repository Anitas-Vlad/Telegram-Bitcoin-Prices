namespace TelegramBitcoinPrices.Interfaces;

public interface ITelegramBitcoinPricesNotifier
{
    Task StartLowHighNotifications(string input);
    Task StopLowHighNotifications();

    Task Run();
}