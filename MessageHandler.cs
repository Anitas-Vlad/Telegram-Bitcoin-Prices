using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegramBitcoinPrices;

// Asta ii clasa ce o folosesc sa scriu mesaje, sa arate mai ok codul
public static class MessageHandler
{
    public static async Task SendMessage(TelegramBotClient telegramBotClient, long chatId, string text)
    {
        try
        {
            // Send a message
            var message = await telegramBotClient.SendMessage(
                chatId: chatId,
                text: text
            );
            Console.WriteLine($"Message sent: {message.Text}");
        }
        catch (ApiRequestException apiRequestException)
        {
            Console.WriteLine($"Error in sending message: {apiRequestException.Message}");
        }
    }
}