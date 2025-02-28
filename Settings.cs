namespace TelegramBitcoinPrices;

public static class Settings
{
    public const string BinanceApiUrl = "https://api.binance.com/api/v3/ticker/price?symbol=BTCEUR";

    public const string TelegramApiUrl = "https://api.telegram.org/bot7950624784:AAHyDDm9nhNR94uBTFe8SSUMTP9ZjkGfWVU/getUpdates?offset=LAST_UPDATE_ID\n";
    public const string TelegramBotToken = "7950624784:AAHyDDm9nhNR94uBTFe8SSUMTP9ZjkGfWVU";
    public const long TelegramChatId = 7600435223;
}