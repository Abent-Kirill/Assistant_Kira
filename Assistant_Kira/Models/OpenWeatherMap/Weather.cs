using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Weather
{
	[JsonPropertyName("main")]
	public Temperature Temperature { get; init; }

	[JsonPropertyName("weather")]
	public Forecast[] DayForecast { get; init; }

	[JsonPropertyName("wind")]
	public Wind Wind { get; init; }

	[JsonPropertyName("name")]
	public string Name { get; init; }

    [JsonConstructor]
    public Weather(Temperature temperature, Forecast[] dayForecast, Wind wind, string name)
    {
        Temperature = temperature;
        DayForecast = dayForecast;
        Wind = wind;
        Name = name;
    }

    public override readonly string ToString() =>
	 $"В городе {Name} сейчас {Temperature.Value}°C ({Temperature.FeelsLikeValue}°C)\n{DayForecast[0]}\n{Wind}";
}
