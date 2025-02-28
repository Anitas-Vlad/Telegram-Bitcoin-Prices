namespace TelegramBitcoinPrices.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(string message);
    Task<string?> ListenForCommands();
    Task InitializeLastUpdateId();
}