using TelegramBitcoinPrices.Interfaces;

namespace TelegramBitcoinPrices.Services;

public class AlertService : IAlertService
{
    private readonly ITelegramService _telegramService;
    
    public AlertService(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task SendCurrentPriceAlert(decimal currentPrice)
    {
        if (currentPrice == 0) return;
        await _telegramService.SendMessageAsync($"Current BTC Price: {currentPrice}");
    }

    public async Task SendBuyTargetAlert(decimal currentPrice)
    {
        if (currentPrice == 0) return;
        await _telegramService.SendMessageAsync($"🔔🔔🔔 BTC: {currentPrice}");
    }

    public async Task SendSellWarningAlert(decimal currentPrice)
    {
        if (currentPrice == 0) return;
        await _telegramService.SendMessageAsync($"🟥🟥🟥 BTC: {currentPrice}");
    }

    public async Task SendSellTargetAlert(decimal currentPrice)
    {
        if (currentPrice == 0) return;
        await _telegramService.SendMessageAsync($"🟩🟩🟩 BTC:{currentPrice}");
    }
}