using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class WeatherCommand(WeatherService weatherService) : ICommand
{
	private readonly WeatherService _weatherService = weatherService;

	public string Name => "погода";

    public string Execute()
	{
		//TODO: Разобраться с result
		var weather = _weatherService.GetWeatherAsync().Result;
		return weather.ToString();
	}
}
