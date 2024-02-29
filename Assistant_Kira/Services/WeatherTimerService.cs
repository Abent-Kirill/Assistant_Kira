
using Assistant_Kira.Models;

using Telegram.Bot;

namespace Assistant_Kira.Services;

internal sealed class WeatherTimerService(ILogger<WeatherService> logger, KiraBot kiraBot, WeatherService weatherService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var plannTime = new TimeOnly(19, 45); //TODO: Можно перенести для начала в appsettings

            if (now.Hour == plannTime.Hour && now.Minute == plannTime.Minute)
            {
                var weatherInfo = await weatherService.GetWeatherAsync();
                _ = await kiraBot.TelegramApi.SendTextMessageAsync(1548307601, weatherInfo.ToString(), cancellationToken: stoppingToken);
                logger.LogInformation("Выполнился таймер о погоде: {Message}", weatherInfo);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
