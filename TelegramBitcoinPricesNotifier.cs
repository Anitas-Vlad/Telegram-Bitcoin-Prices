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

    decimal buyPoint = 83700; // = 0
    decimal sellSafePoint = 83500; // = 0
    decimal sellTargetPoint = 83880; // = 0

    public decimal ActiveLowestPrice = 0;
    public decimal ActiveHighestPrice = 0;

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

        // Calculate the formula based on the input bitcoin price and automatically set up minimum maximum limits for max loss and minim earn
        // ApplyFormula(); // Input based on your wishes

        await _telegramService.SendMessageAsync("Bitcoin Price Tracker started...");

        while (isActive)
        {
            currentPrice = await _btcPriceService.GetBitcoinPriceAsync();
            CheckBtcPrice(currentPrice);

            switch (AwaitedStatusType)
            {
                case StatusType.Buy:
                {
                    await _alertService.SendBuyTargetAlert(currentPrice);
                    break;
                }
                case StatusType.SellTarget:
                {
                    await _alertService.SendSellTargetAlert(currentPrice);
                    break;
                }
                case StatusType.SellSafe:
                {
                    await _alertService.SendSellWarningAlert(currentPrice);
                    break;
                }
                case StatusType.Skip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(TimeSpan.FromSeconds(1)); // Periodical check
        }

        // while (true)
        // {
        //     await _alertService.SendCurrentPriceAlert(currentPrice);
        //     await Task.Delay(5000);
        // }
    }

    private void CheckBtcPrice(decimal currentPrice)
    {
        // SKIP in case the highest and lowest both have values and the current value does not exceed them.
        if (ActiveLowestPrice != 0 && ActiveHighestPrice != 0 &&
            ActiveLowestPrice < currentPrice && currentPrice < ActiveHighestPrice)
        {
            AwaitedStatusType = StatusType.Skip;
            return;
        }
        // SKIP in case the highest and lowest both have values and the current value does not exceed them.

        // From Scratch

        if (currentPrice >= ActiveHighestPrice)
        {
            AwaitedStatusType = StatusType.SellTarget;
            ActiveHighestPrice = this.currentPrice;
            return;
        }

        if (currentPrice >= sellTargetPoint)
        {
            if (currentPrice < ActiveHighestPrice)
            {
                AwaitedStatusType = StatusType.Skip;
                return;
            }

            AwaitedStatusType = StatusType.SellTarget;
        }

        if (sellTargetPoint > currentPrice && currentPrice > sellSafePoint)
        {
            AwaitedStatusType = StatusType.Skip;
        }

        if (currentPrice <= sellSafePoint)
        {
            if (currentPrice > ActiveLowestPrice)
            {
                AwaitedStatusType = StatusType.Skip;
                return;
            }

            AwaitedStatusType = StatusType.Buy;
            return;
        }

        if (currentPrice < ActiveLowestPrice)
        {
            AwaitedStatusType = StatusType.Buy;
            ActiveLowestPrice = currentPrice;
        }
    }

    public void ApplyFormula(decimal btcPrice, decimal maxLossEuro, decimal mimEarnEuro)
        // formula based on min max earnings
    {
    }

    public void applyFormula2(decimal btcPrice, decimal maxLossBtcPrice, decimal minEarnBtcPrice)
        // formula based on btc movement
    {
    }

    public async Task Stop() => isActive = false;
}