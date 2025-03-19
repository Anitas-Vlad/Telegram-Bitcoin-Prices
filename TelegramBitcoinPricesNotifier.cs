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
    public readonly IInputParserService _inputParserService;

    private bool isLowHighActive;
    private bool isTrackerActive;
    private bool isEmaActive;

    public decimal _buyPoint;

    private BtcPriceStatus AwaitedBtcPriceStatus = BtcPriceStatus.Skip;
    private decimal _currentPrice;
    private decimal _highPriceBorder;
    private decimal _lowPriceBorder;

    private LimitedQueue _emaQueue; //seconds for now
    private decimal _currentEmaPrice; //seconds for now
    private decimal _lowestEma;
    private decimal _highestEma;


    public TelegramBitcoinPricesNotifier(IBtcPriceService btcPriceService, ITelegramService telegramService,
        IAlertService alertService, IInputParserService inputParserService)
    {
        isLowHighActive = true;
        _btcPriceService = btcPriceService;
        _telegramService = telegramService;
        _alertService = alertService;
        _inputParserService = inputParserService;
        _emaQueue = new LimitedQueue(200);
    }

    public async Task Run()
    {
        await _telegramService.InitializeLastUpdateId();

        while (true)
        {
            var message = await _telegramService.ListenForCommands();

            switch (message)
            {
                case "track btc":
                    await StartBtcTracker();
                    break;
                case "stats":
                    await GetStats();
                    break;
                case "stop tracker":
                    StopTracker();
                    break;
                case "stop low high":
                    await StopLowHighNotifications();
                    break;

                default:
                {
                    if (message.StartsWith("start"))
                        await StartLowHighNotifications(message);
                    else if (message.StartsWith("buy"))
                        await SetBuyPoint(message);
                    break;
                }
            }

            await Task.Delay(5000);
        }
    }

    private async Task StartBtcTracker() //TODO take an argument with the time so it's more dynamic for multiple ema-s
    {
        try
        {
            await _telegramService.SendMessageAsync("BTC Tracker Running...");
            isTrackerActive = true;

            _ = Task.Run(async () =>
            {
                while (isTrackerActive)
                {
                    _currentPrice = await _btcPriceService.GetBitcoinPriceAsync();
                    UpdateStats(_currentPrice);

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
            $"BTC: {Math.Round(_currentPrice, 5, MidpointRounding.AwayFromZero)} Euro\n" +
            $"EMA for {_emaQueue.Count} sec: {Math.Round(_currentEmaPrice, 5, MidpointRounding.AwayFromZero)}");
    }

    private void UpdateStats(decimal currentBtcPrice)
    {
        _currentPrice = currentBtcPrice;
        _emaQueue.Enqueue(currentBtcPrice);

        _currentEmaPrice = _emaQueue.Average();

        if (_lowestEma == 0 && _highestEma == 0)
        {
            _lowestEma = _currentEmaPrice;
            _highestEma = _currentEmaPrice;
        }

        if (_currentEmaPrice < _lowestEma)
        {
            _lowestEma = _currentEmaPrice;
        }

        if (_highestEma < _currentEmaPrice)
        {
            _highestEma = _currentEmaPrice;
        }

        Console.WriteLine(_currentPrice);
    }

    public async Task StartLowHighNotifications(string input)
    {
        try
        {
            ApplyMinMaxBtcPrice(input);

            await _telegramService.SendMessageAsync("Successfully activated low high notifications...");
            isLowHighActive = true;
            _ = Task.Run(async () =>
            {
                while (isLowHighActive)
                {
                    CheckBtcPrice(_currentPrice);

                    switch (AwaitedBtcPriceStatus)
                    {
                        case BtcPriceStatus.Buy:
                        {
                            await _alertService.SendBuyTargetAlert(_currentPrice);
                            break;
                        }
                        case BtcPriceStatus.Rise:
                        {
                            await _alertService.SendSellTargetAlert(_currentPrice);
                            break;
                        }
                        case BtcPriceStatus.Drop:
                        {
                            await _alertService.SendSellWarningAlert(_currentPrice);
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

    public async Task StopTracker()
    {
        isTrackerActive = false;
        Console.WriteLine("Btc tracker stopped...");
        await _telegramService.SendMessageAsync("Btc tracker stopped...");
    }

    public async Task StopLowHighNotifications()
    {
        isLowHighActive = false;
        _lowPriceBorder = 0;
        _highPriceBorder = 0;
        Console.WriteLine("Low high notifications stopped...");
        await _telegramService.SendMessageAsync("Low high notifications stopped...");
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
        if (currentPrice > _highPriceBorder)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Rise;
            _highPriceBorder = currentPrice;
            return;
        }

        if (currentPrice < _lowPriceBorder)
        {
            AwaitedBtcPriceStatus = BtcPriceStatus.Drop;
            _lowPriceBorder = currentPrice;
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

        _lowPriceBorder = range.LowBtcPrice;
        _highPriceBorder = range.HighBtcPrice;

        _telegramService.SendMessageAsync($"Low Number: {range.LowBtcPrice}, High Number: {range.HighBtcPrice}");
    }
}