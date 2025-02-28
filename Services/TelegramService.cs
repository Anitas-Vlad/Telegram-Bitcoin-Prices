using System.Text.Json;
using Telegram.Bot;
using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.TelegramModels;

namespace TelegramBitcoinPrices.Services;

public class TelegramService : ITelegramService
{
    private static int _lastUpdateId;
    private readonly TelegramBotClient _botClient;
    private readonly HttpClient _httpClient;

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

    public async Task InitializeLastUpdateId()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{Settings.TelegramApiUrl}/getUpdates");
            var updates = JsonSerializer.Deserialize<UpdateResponse>(response);

            if (updates?.result != null && updates.result.Length > 0)
            {
                _lastUpdateId = updates.result[^1].update_id; // Set to the last update ID
                Console.WriteLine($"Initialized _lastUpdateId: {_lastUpdateId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing last update ID: {ex.Message}");
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

            if (updates?.result != null && updates.result.Length > 0)
            {
                var lastUpdate = updates.result[^1]; // Get the latest message only

                if (lastUpdate.update_id == _lastUpdateId)
                    return "";

                if (lastUpdate.update_id > _lastUpdateId)
                {
                    _lastUpdateId = lastUpdate.update_id; // Mark it as processed

                    message = lastUpdate.message.text.Trim().ToLower();

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