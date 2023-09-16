using Assistant_Kira.Services;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand : ICommand
{
	private readonly WeatherService _weatherService;

	public string Name => "погода";

	public WeatherCommand(WeatherService weatherService)
	{
		_weatherService = weatherService;
	}

	public string Execute()
	{
		var weather = _weatherService.GetWeatherAsync().Result;
		return weather.Temperature.Value.ToString();
	}
}
