namespace TelegramBitcoinPrices.TelegramModels;

public class Chat
{
    public long id { get; set; }
    public string first_name { get; set; } = default!;
    public string type { get; set; } = default!;
}