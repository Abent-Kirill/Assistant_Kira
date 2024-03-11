using System.Text;

using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand(WeatherService weatherService, KiraBot kiraBot, ILogger<WeatherCommand> logger) : ICommand
{
    public string Name => "погода";

	public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        string textMessage;

        try
        {
            if (args == null || args.All(string.IsNullOrWhiteSpace))
            {
                var weather = await weatherService.GetWeatherAsync();
                await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, weather.ToString());
                return;
            }

            var cites = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase));
            var strBuilder = new StringBuilder();

            foreach (var arg in cites)
            {
                var weather = await weatherService.GetWeatherAsync(arg);
                strBuilder.AppendLine(weather.ToString());
            }
            textMessage = strBuilder.ToString();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            textMessage = ex.Message;
        }
		await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, textMessage);
	}
}
