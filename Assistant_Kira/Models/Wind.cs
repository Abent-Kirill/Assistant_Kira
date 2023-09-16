using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

internal struct Wind
{
	[JsonPropertyName(name: "speed")]
	public float Speed { get; set; }
}