namespace TelegramBitcoinPrices.Interfaces;

public interface IAlertService
{
    Task SendCurrentPriceAlert(decimal currentPrice);

    Task SendBuyTargetAlert(decimal currentPrice);

    Task SendSellWarningAlert(decimal currentPrice);

    Task SendSellTargetAlert(decimal currentPrice);
}