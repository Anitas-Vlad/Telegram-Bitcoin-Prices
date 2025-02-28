using TelegramBitcoinPrices.Enums;
using TelegramBitcoinPrices.Input;
using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices;

public class TelegramBitcoinPricesNotifier : ITelegramBitcoinPricesNotifier
{
    public bool isActive = false;
    public readonly IBtcPriceService _btcPriceService;
    public readonly ITelegramService _telegramService;
    public readonly IAlertService _alertService;
    public readonly IInputParser _inputParser;

    // decimal buyPoint = 83700; // = 0
    decimal sellSafePoint = 0; // = 0
    decimal sellTargetPoint = 0; // = 0

    public decimal ActiveLowestPrice = 0;
    public decimal ActiveHighestPrice = 0;

    public BtcPriceStatus AwaitedBtcPriceStatus = BtcPriceStatus.Skip;

    decimal currentPrice = 0;

    public TelegramBitcoinPricesNotifier(IBtcPriceService btcPriceService, ITelegramService telegramService,
        IAlertService alertService, IInputParser inputParser)
    {
        isActive = true;
        _btcPriceService = btcPriceService;
        _telegramService = telegramService;
        _alertService = alertService;
        _inputParser = inputParser;
    }

    public async Task Run()
    {
        while (true)
        {
            var message = await _telegramService.ListenForCommands();

            if (message.StartsWith("start"))
                await Start(message);
            else if (message.StartsWith("stop"))
                await Stop();

            await Task.Delay(5000);
        }
    }

    public async Task Start(string input)
    {
        try
        {
            ApplyMinMaxBtcPriceFormula(input);

            await _telegramService.SendMessageAsync("Bitcoin Price Tracker started...");
            isActive = true;
            _ = Task.Run(async () =>
            {
                while (isActive)
                {
                    currentPrice = await _btcPriceService.GetBitcoinPriceAsync();
                    CheckBtcPrice(currentPrice);

                    switch (AwaitedBtcPriceStatus)
                    {
                        case BtcPriceStatus.Buy:
                        {
                            await _alertService.SendBuyTargetAlert(currentPrice);
                            break;
                        }
                        case BtcPriceStatus.Rise:
                        {
                            await _alertService.SendSellTargetAlert(currentPrice);
                            break;
                        }
                        case BtcPriceStatus.Drop:
                        {
                            await _alertService.SendSellWarningAlert(currentPrice);
                            break;
                        }
                        case BtcPriceStatus.Skip:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }
        catch (Exception e)
        {
            await _telegramService.SendMessageAsync($"Error: {e.Message}");
        }
    }

    private void CheckBtcPrice(decimal currentPrice)
    {
        if (currentPrice > ActiveHighestPrice)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Rise;
            ActiveHighestPrice = currentPrice;
            return;
        }

        if (currentPrice < ActiveLowestPrice)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Drop;
            ActiveLowestPrice = currentPrice;
            return;
        }
        AwaitedBtcPriceStatus = BtcPriceStatus.Skip;
    }

    public void ApplyFormula(decimal btcPrice, decimal maxLossEuro, decimal mimEarnEuro)
        // formula based on min max earnings
    {
    }

    private void ApplyMinMaxBtcPriceFormula(string input)
        // formula based on btc movement
    {
        var parser = new InputParser();
        var range = parser.ParseBtcRange(input);

        sellSafePoint = range.LowBtcPrice;
        sellTargetPoint = range.HighBtcPrice;

        ActiveLowestPrice = range.LowBtcPrice;
        ActiveHighestPrice = range.HighBtcPrice;

        _telegramService.SendMessageAsync($"Low Number: {range.LowBtcPrice}, High Number: {range.HighBtcPrice}");
    }

    public async Task Stop()
    {
        isActive = false;
        sellSafePoint = 0;
        sellTargetPoint = 0;
        Console.WriteLine("Tracker stopped...");
        await _telegramService.SendMessageAsync("Tracker stopped...");
    }
}