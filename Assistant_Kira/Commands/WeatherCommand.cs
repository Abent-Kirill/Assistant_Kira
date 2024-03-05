using System.Text;

using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand(WeatherService weatherService) : ICommand
{
    public string Name => "погода";

	public async Task<string> ExecuteAsync(IEnumerable<string> args)
	{
		if (args == null || args.All(string.IsNullOrWhiteSpace))
		{
            var weather = await weatherService.GetWeatherAsync();
            return weather.ToString();
		}

		var cites = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase));
		var strBuilder = new StringBuilder();

		foreach (var arg in cites)
		{
            var weather = await weatherService.GetWeatherAsync(arg);
            strBuilder.AppendLine(weather.ToString());
		}
		return strBuilder.ToString();
	}
}
