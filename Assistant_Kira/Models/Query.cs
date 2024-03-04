using System.Text.Json.Serialization;

namespace Assistant_Kira.Models;

internal struct Query
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("form")]
    public string From { get; set; }
    [JsonPropertyName("to")]
    public string To { get; set; }
}
