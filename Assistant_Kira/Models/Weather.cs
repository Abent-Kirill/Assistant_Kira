using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

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
}
