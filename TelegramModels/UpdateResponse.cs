namespace TelegramBitcoinPrices.TelegramModels;

public class UpdateResponse
{
    public bool ok { get; set; } // To capture the "ok" status
    public Update[]? result { get; set; }
}