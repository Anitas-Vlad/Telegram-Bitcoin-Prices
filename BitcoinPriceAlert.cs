using System.Text.Json;
using RestSharp;
using TelegramBitcoinPrices.Responses;

namespace TelegramBitcoinPrices;

public class BitcoinPriceAlert
{
    // private const string BinanceApiUrl = "https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT";
    // private decimal lastPrice;
    //
    // static async Task<string> GetBitcoinPriceAsync()
    // {
    //     using var client = new HttpClient();
    //     var response = await client.GetStringAsync("https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT");
    //
    //     var priceData = JsonSerializer.Deserialize<PriceResponse>(response);
    //     return priceData.Price;
    // }
    //
    // public async Task CheckPriceAsync(decimal dipThreshold)
    // {
    //     var client = new RestClient(BinanceApiUrl);
    //     var request = new RestRequest(Method.Get);
    //     
    //     var response = await client.ExecuteAsync(request);
    //     if (response.IsSuccessful)
    //     {
    //         var json = response.Content;
    //         var currentPrice = decimal.Parse(JsonDocument.Parse(json).RootElement.GetProperty("price").GetString());
    //
    //         if (lastPrice != 0 && (lastPrice - currentPrice) / lastPrice >= dipThreshold)
    //         {
    //             Console.WriteLine($"Price dipped! New price: {currentPrice}");
    //             // Here you can add logic to send an alert (email, SMS, etc.)
    //         }
    //
    //         lastPrice = currentPrice; // Update last price
    //     }
    //     else
    //     {
    //         Console.WriteLine("Error fetching price.");
    //     }
    // }
}