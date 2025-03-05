
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBitcoinPrices;
using TelegramBitcoinPrices.Input;
using TelegramBitcoinPrices.Interfaces;
using TelegramBitcoinPrices.Services;

using var host = CreateHostBuilder(args).Build();
var telegramBitcoinPricesNotifier = host.Services.GetRequiredService<ITelegramBitcoinPricesNotifier>();

await telegramBitcoinPricesNotifier.Run();

return;

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IBtcPriceService, BtcPriceService>();
            services.AddSingleton<ITelegramService, TelegramService>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<ITelegramBitcoinPricesNotifier, TelegramBitcoinPricesNotifier>();
            services.AddSingleton<IInputParserService, InputParserServiceServiceService>();
        });
}