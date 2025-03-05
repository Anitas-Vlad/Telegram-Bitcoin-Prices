using TelegramBitcoinPrices.Enums;
using TelegramBitcoinPrices.Input;
using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.Models;

namespace TelegramBitcoinPrices;

public class TelegramBitcoinPricesNotifier : ITelegramBitcoinPricesNotifier
{
    public readonly IAlertService _alertService;
    public readonly IBtcPriceService _btcPriceService;
    public readonly ITelegramService _telegramService;
    public readonly IInputParserService InputParserService;

    public bool isLowHighActive;
    public bool isEMAActive;
    
    public decimal _buyPoint;

    public BtcPriceStatus AwaitedBtcPriceStatus = BtcPriceStatus.Skip;
    private decimal currentPrice;
    public decimal HighPriceBorder;

    public LimitedQueue EMAQueue; //seconds for now
    public decimal EMAPrice; //seconds for now
    private decimal _currentPrice;

    public decimal LowPriceBorder;


    public TelegramBitcoinPricesNotifier(IBtcPriceService btcPriceService, ITelegramService telegramService,
        IAlertService alertService, IInputParserService inputParserService)
    {
        isLowHighActive = true;
        _btcPriceService = btcPriceService;
        _telegramService = telegramService;
        _alertService = alertService;
        InputParserService = inputParserService;
        EMAQueue = new LimitedQueue(100);
    }

    public async Task Run()
    {
        await _telegramService.InitializeLastUpdateId();

        while (true)
        {
            var message = await _telegramService.ListenForCommands();

            switch (message)
            {
                case "start ema":
                    await StartEMA();
                    break;
                case "stats":
                    await GetStats();
                    break;
                default:
                {
                    if(message.StartsWith("start"))
                        await StartLowHighNotifications(message);
                    else if (message.StartsWith("stop"))
                        await StopLowHighNotifications();
                    else if (message.StartsWith("buy")) 
                        await SetBuyPoint(message);
                    break;
                }
            }

            await Task.Delay(5000);
        }
    }
    
    private async Task StartEMA()
    {
        try
        {
            await _telegramService.SendMessageAsync("EMA Running...");
            isEMAActive = true;

            _ = Task.Run(async () =>
            {
                while (isLowHighActive)
                {
                    var currentBtcPrice = await _btcPriceService.GetBitcoinPriceAsync();
                    UpdateStats(currentBtcPrice);

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }
        catch (Exception e)
        {
            await _telegramService.SendMessageAsync($"Error: {e.Message}");
        }
    }
    
    private async Task GetStats()
    {
        await _telegramService.SendMessageAsync(
            "Bitcoin stats:\n" +
            $"BTC: {_currentPrice} Euro\n" +
            $"EMA for {EMAQueue.Count} Min: {EMAPrice}");
    }
    
    private void UpdateStats(decimal currentBtcPrice)
    {
        _currentPrice = currentBtcPrice;
        EMAQueue.Enqueue(currentBtcPrice);
        EMAPrice = EMAQueue.Average();
    }

    public async Task StartLowHighNotifications(string input)
    {
        try
        {
            ApplyMinMaxBtcPrice(input);

            await _telegramService.SendMessageAsync("Bitcoin Price Tracker started...");
            isLowHighActive = true;
            _ = Task.Run(async () =>
            {
                while (isLowHighActive)
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

    public async Task StopLowHighNotifications()
    {
        isLowHighActive = false;
        LowPriceBorder = 0;
        HighPriceBorder = 0;
        Console.WriteLine("Tracker stopped...");
        await _telegramService.SendMessageAsync("Tracker stopped...");
    }

    private async Task SetBuyPoint(string message)
    {
        var buyPoint = InputValidator.ValidateAndExtractNumber(message);

        if (buyPoint == 0)
        {
            await _telegramService.SendMessageAsync($"Invalid buy price: {buyPoint}");
            return;
        }

        _buyPoint = buyPoint;
        await _telegramService.SendMessageAsync($"Buy price: {buyPoint}");
    }

    private void CheckBtcPrice(decimal currentPrice)
    {
        if (currentPrice > HighPriceBorder)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Rise;
            HighPriceBorder = currentPrice;
            return;
        }

        if (currentPrice < LowPriceBorder)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Drop;
            LowPriceBorder = currentPrice;
            return;
        }

        AwaitedBtcPriceStatus = BtcPriceStatus.Skip;
    }

    public void ApplyFormula(decimal btcPrice, decimal maxLossEuro, decimal mimEarnEuro)
        // formula based on min max earnings
    {
    }

    private void ApplyMinMaxBtcPrice(string input)
    {
        var parserService = new InputParserServiceServiceService();
        var range = parserService.ParseBtcRange(input);

        LowPriceBorder = range.LowBtcPrice;
        HighPriceBorder = range.HighBtcPrice;

        _telegramService.SendMessageAsync($"Low Number: {range.LowBtcPrice}, High Number: {range.HighBtcPrice}");
    }
}