namespace TelegramBitcoinPrices.Interfaces;

public interface IBtcPriceService
{
    Task<decimal> GetBitcoinPriceAsync();
}