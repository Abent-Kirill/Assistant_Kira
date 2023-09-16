using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

internal struct Forecast
{
	[JsonPropertyName("main")]
	public string Value { get; set; }

	[JsonPropertyName(name: "description")]
	public string Description { get; set; }
}
