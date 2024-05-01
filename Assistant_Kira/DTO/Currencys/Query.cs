using System.Text.Json.Serialization;

namespace Assistant_Kira.DTO.Currencys;

internal struct Query
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    [JsonPropertyName("from")]
    public string From { get; init; }
    [JsonPropertyName("to")]
    public string To { get; init; }

    [JsonConstructor]
    public Query(decimal amount, string from, string to)
    {
        Amount = amount;
        From = from;
        To = to;
    }
}
