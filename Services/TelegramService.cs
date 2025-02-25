using Telegram.Bot;
using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices.Services;

public class TelegramService : ITelegramService
{
    private readonly TelegramBotClient _botClient;

    public TelegramService()
    {
        _botClient = new TelegramBotClient(Settings.TelegramBotToken);
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await _botClient.SendMessage(Settings.TelegramChatId, message);
            Console.WriteLine("Telegram alert sent!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending Telegram message: {ex.Message}");
        }
    }
}