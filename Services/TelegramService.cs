using System.Text.Json;
using Telegram.Bot;
using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.Responses;
using TelegramBitcoinPrices.TelegramModels;

namespace TelegramBitcoinPrices.Services;

public class TelegramService : ITelegramService
{
    private readonly TelegramBotClient _botClient;
    private readonly HttpClient _httpClient;
    private static int _lastUpdateId = 0;

    public TelegramService()
    {
        _botClient = new TelegramBotClient(Settings.TelegramBotToken);
        _httpClient = new HttpClient();
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await _botClient.SendMessage(Settings.TelegramChatId, message);
            Console.WriteLine("SENT --- " + message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending Telegram message: {ex.Message}");
        }
    }

    public async Task<string?> ListenForCommands()
    {
        var message = "";
        try
        {
            var response =
                await _httpClient.GetStringAsync($"{Settings.TelegramApiUrl}/getUpdates?offset={_lastUpdateId + 1}");
            var updates = JsonSerializer.Deserialize<UpdateResponse>(response);

            // Console.WriteLine(updates);

            if (updates?.result != null && updates.result.Length > 0)
            {
                var lastUpdate = updates.result[^1]; // Get the latest message only

                if (lastUpdate.update_id > _lastUpdateId)
                {
                    _lastUpdateId = lastUpdate.update_id; // Mark it as processed

                    message = lastUpdate.message.text.Trim().ToLower();

                    // return string.IsNullOrEmpty(message) ? "" : message;
                    return message;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching updates: {ex.Message}");
        }

        return "";
    }
}