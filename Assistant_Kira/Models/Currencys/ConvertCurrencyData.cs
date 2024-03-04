using System.Text.Json.Serialization;

namespace Assistant_Kira.Models.Currencys;

internal struct ConvertCurrencyData
{
    private decimal _result;

    [JsonPropertyName("result")]
    public decimal Result
    {
        readonly get => decimal.Round(_result, 2);
        set => _result = value;
    }
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    [JsonPropertyName("query")]
    public Query QueryInfo { get; set; }

    public override readonly string ToString() => $"{QueryInfo.Amount} {QueryInfo.From} = {Result} {QueryInfo.To}";
}
