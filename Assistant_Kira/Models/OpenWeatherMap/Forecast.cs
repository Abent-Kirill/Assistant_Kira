using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.OpenWeatherMap;

internal struct Forecast
{
	[JsonPropertyName("main")]
	public string Value { get; init; }

	[JsonPropertyName("description")]
	public string Description { get; init; }

    [JsonConstructor]
    public Forecast(string value, string description)
    {
        Value = value;
        Description = description;
    }

    public override readonly string ToString() => Description;
}
