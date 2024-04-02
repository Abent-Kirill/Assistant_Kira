using Telegram.Bot;

namespace Assistant_Kira.Services.WeatherServices;

internal sealed class WeatherTimerService(ILogger<WeatherService> logger, ITelegramBotClient botClient, IWeatherService weatherService,
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
                var weatherInfo = await weatherService.GetWeatherAsync("Samara");
                var chatId = configuration["BotSettings:ChatId"];
                ArgumentException.ThrowIfNullOrWhiteSpace(chatId);
                _ = await botClient.SendTextMessageAsync(chatId, weatherInfo.ToString(), cancellationToken: stoppingToken);
                logger.LogInformation("Выполнился таймер о погоде: {Message}", weatherInfo);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
