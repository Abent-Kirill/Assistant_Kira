using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Weather
{
	[JsonPropertyName("main")]
	public Temperature Temperature { get; set; }

	[JsonPropertyName("weather")]
	public Forecast[] DayForecast { get; set; }

	[JsonPropertyName("wind")]
	public Wind Wind { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	public override readonly string ToString() =>
	 $"В городе {Name} сейчас {Temperature.Value}°C ({Temperature.FeelsLikeValue}°C)\n{DayForecast[0]}\n{Wind}";
}
