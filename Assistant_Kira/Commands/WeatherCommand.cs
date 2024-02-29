using System.Text;

using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand(WeatherService weatherService) : ICommand
{
    public string Name => "погода";

	public string Execute(IEnumerable<string> args)
	{
		//TODO: Разобраться с result
		if (args == null || args.All(string.IsNullOrWhiteSpace))
		{
			return weatherService.GetWeatherAsync().Result.ToString();
		}
		var cites = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase));
		var strBuilder = new StringBuilder();
		foreach (var arg in cites)
		{
			strBuilder.AppendLine(weatherService.GetWeatherAsync(arg).Result.ToString());
		}
		return strBuilder.ToString();
	}
}
