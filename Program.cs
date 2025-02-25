using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.Services;

decimal lastPrice = 0;
var DipThreshold = 0.02m; // 2% drop

Console.WriteLine("Bitcoin Price Tracker started...");

var httpClient = new HttpClient();
IBtcPriceService priceService = new BtcPriceService(httpClient);
ITelegramService telegramService = new TelegramService();

while (true)
{
    await CheckPriceAsync(priceService, telegramService);
    await Task.Delay(TimeSpan.FromMinutes(1)); // Check every minute
}

async Task CheckPriceAsync(IBtcPriceService priceService, ITelegramService telegramService)
{
    var currentPrice = await priceService.GetBitcoinPriceAsync();
    if (currentPrice == 0) return; // Skip if error occurred

    Console.WriteLine($"Current BTC Price: ${currentPrice}");

    if (lastPrice != 0 && (lastPrice - currentPrice) / lastPrice >= DipThreshold)
    {
        var message =
            $"🚨 Bitcoin Price Drop Alert! 🚨\nNew Price: ${currentPrice}\nPrevious Price: ${lastPrice}\nDrop: {DipThreshold * 100}%";
        await telegramService.SendMessageAsync(message);
    }

    lastPrice = currentPrice;
}