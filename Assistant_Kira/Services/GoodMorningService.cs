using Assistant_Kira.Options;
using Assistant_Kira.Requests;

using MediatR;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Assistant_Kira.Services;

internal sealed class GoodMorningService(ILogger<GoodMorningService> logger, ITelegramBotClient botClient,
                IOptions<BotOptions> configuration, IMediator mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            if (now.Hour == 10 && now.Minute == 0)
            {
                var weatherInfo = await mediator.Send(new WeatherRequest("Samara"), stoppingToken);
                var events = await mediator.Send(new EventsRequest(DateTimeOffset.Now), stoppingToken);
                var eventsStr = "На сегодня событий не запланировано!";
                var chatId = configuration.Value.ChatId;

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
