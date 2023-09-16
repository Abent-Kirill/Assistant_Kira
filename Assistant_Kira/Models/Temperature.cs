using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

internal struct Temperature
{
	[JsonPropertyName("temp")]
	public float Value { get; set; }

	[JsonPropertyName("feels_like")]
	public float FeelsLikeValue { get; set; }
}
