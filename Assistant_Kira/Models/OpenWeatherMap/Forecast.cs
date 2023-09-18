using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Forecast
{
	[JsonPropertyName("main")]
	public string Value { get; set; }

	[JsonPropertyName(name: "description")]
	public string Description { get; set; }

	public override readonly string ToString() => Description;
}
