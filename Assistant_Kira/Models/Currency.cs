using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

internal struct Currency
{
	[JsonPropertyName("base")]
	public string Name { get; set; }

	[JsonPropertyName("rates")]
	public IDictionary<string, decimal> Rates { get; set; }
}