using System.Text;

using Assistant_Kira.Services.WeatherServices;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand(IWeatherService weatherService, ILogger<WeatherCommand> logger) : Command
{
    public override string Name => "погода";

	public override async Task<string> ExecuteAsync(params string[] args)
    {
        string textMessage;

        try
        {
            if (args == null || args.All(string.IsNullOrWhiteSpace))
            {
                var weather = await weatherService.GetWeatherAsync("Samara");
                return weather.ToString();
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
            logger.LogError(ex, "Входные данные: {args}", args);
            textMessage = ex.Message;
        }
		return textMessage;
	}
}
