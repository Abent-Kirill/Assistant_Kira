using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Currencys;

internal struct Query
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("from")]
    public string From { get; set; }
    [JsonPropertyName("to")]
    public string To { get; set; }
}
