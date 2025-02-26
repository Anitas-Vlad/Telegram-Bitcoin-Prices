using System.Text.Json;
using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.Responses;

namespace TelegramBitcoinPrices.Services;

public class BtcPriceService : IBtcPriceService
{
    private readonly HttpClient _httpClient;

    public BtcPriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetBitcoinPriceAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(Settings.BinanceApiUrl);
            var priceData = JsonSerializer.Deserialize<PriceResponse>(response);
            return decimal.Parse(priceData.price);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Bitcoin price: {ex.Message}");
            return 0;
        }
    }
    
    
}