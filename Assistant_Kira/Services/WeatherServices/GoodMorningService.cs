using Assistant_Kira.Services.CalendarServices;

using Telegram.Bot;

namespace Assistant_Kira.Services.WeatherServices;

internal sealed class GoodMorningService(ILogger<WeatherService> logger, ITelegramBotClient botClient, IWeatherService weatherService,
    ICalendarService calendarService, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            if (now.Hour == 10 && now.Minute == 0)
            {
                var weatherInfo = await weatherService.GetWeatherAsync("Samara");
                var events = await calendarService.GetEvents(DateTimeOffset.Now);
                var eventsStr = "На сегодня событий не запланировано!";
                var chatId = configuration["BotSettings:ChatId"];
                ArgumentException.ThrowIfNullOrWhiteSpace(chatId);

                if (events.Count > 0)
                {
                    eventsStr = "Предстоящие события:";
                    foreach (var eventT in events)
                    {
                        eventsStr += $"- {eventT}";
                    }
                }
                
                var message = $"Доброе утро!\n{eventsStr}\nПогода на сегодня:\n{weatherInfo}";
                await botClient.SendTextMessageAsync(chatId, message, cancellationToken: stoppingToken);
                logger.LogInformation("Выполнился таймер о погоде: {Message}", message);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
