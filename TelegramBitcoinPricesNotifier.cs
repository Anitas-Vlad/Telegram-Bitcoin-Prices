using TelegramBitcoinPrices.Enums;
using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices;

public class TelegramBitcoinPricesNotifier : ITelegramBitcoinPricesNotifier
{
    public bool isActive;
    public readonly IBtcPriceService _btcPriceService;
    public readonly ITelegramService _telegramService;
    public readonly IAlertService _alertService;

    // public bool buyAlert = true;
    // public bool sellAlert = false;
    // public bool safeSellAlert = false;

    decimal buyAlertPoint = 82000;
    decimal safeSellPoint = 81950;
    decimal sellAlertPoint = 82100;

    public decimal ActiveLowestPrice = 0;

    public StatusType AwaitedStatusType = StatusType.Buy;

    decimal currentPrice = 0;

    public TelegramBitcoinPricesNotifier(IBtcPriceService btcPriceService, ITelegramService telegramService,
        IAlertService alertService)
    {
        isActive = true;
        _btcPriceService = btcPriceService;
        _telegramService = telegramService;
        _alertService = alertService;
    }

    public async Task Start() // Start(decimal buyAlertPoint, decimal safeSellPoint, decimal sellAlertPoint)
    {
        Console.WriteLine("Bitcoin Price Tracker started...");
        await _telegramService.SendMessageAsync("Bitcoin Price Tracker started...");

        while (isActive)
        {
            currentPrice = await _btcPriceService.GetBitcoinPriceAsync();

            switch (AwaitedStatusType)
            {
                case StatusType.Buy:
                {
                    if (currentPrice > ActiveLowestPrice)
                        break;
                    
                    await _alertService.SendBuyTargetAlert(currentPrice);
                    ActiveLowestPrice = currentPrice;

                    break;
                }
                case StatusType.SellTarget:
                {
                    break;
                }
                case StatusType.SellSafe:
                {
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(TimeSpan.FromSeconds(5)); // Periodical check
        }

        // while (true)
        // {
        //     await _alertService.SendCurrentPriceAlert(currentPrice);
        //     await Task.Delay(5000);
        // }
    }

    public async Task CheckBtcPrice(decimal currentPrice)
    {
        if (currentPrice <= buyAlertPoint)
        {
            AwaitedStatusType = StatusType.Buy;
        }
    }

    public async Task Stop() => isActive = false;
}