using System.Text.Json.Serialization;

namespace Assistant_Kira.DTO.Currencys;

internal struct Currency
{
    [JsonPropertyName("base")]
    public string Name { get; init; }

    [JsonPropertyName("rates")]
    public IDictionary<string, decimal> Rates { get; init; }

    [JsonConstructor]
    public Currency(string name, IDictionary<string, decimal> rates)
    {
        Name = name;
        Rates = rates;
    }
}
