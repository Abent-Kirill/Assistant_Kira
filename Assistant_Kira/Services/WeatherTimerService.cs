using Assistant_Kira.Models;

using Telegram.Bot;

namespace Assistant_Kira.Services;

internal sealed class WeatherTimerService(ILogger<WeatherService> logger, KiraBot kiraBot, WeatherService weatherService,
    IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            //TODO: Можно перенести для начала в appsettings

            if (now.Hour == 10 && now.Minute == 0)
            {
                var weatherInfo = await weatherService.GetWeatherAsync();
                var chatId = configuration["BotSettings:ChatId"];
                ArgumentNullException.ThrowIfNullOrWhiteSpace(chatId);
                _ = await kiraBot.SendTextMessageAsync(chatId, weatherInfo.ToString(), cancellationToken: stoppingToken);
                logger.LogInformation("Выполнился таймер о погоде: {Message}", weatherInfo);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
