namespace TelegramBitcoinPrices.Interfaces;

public interface IAlertService
{
    Task SendCurrentPriceAlert(decimal currentPrice);

    Task SendBuyTargetAlert(decimal currentPrice);

    Task SendSellWarningAlert(decimal currentPrice);

    Task SendTargetSellAlert(decimal currentPrice);
}