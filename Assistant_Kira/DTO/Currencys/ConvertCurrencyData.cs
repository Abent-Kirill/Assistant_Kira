using System.Text.Json.Serialization;

namespace Assistant_Kira.DTO.Currencys;

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
    public bool Success { get; init; }
    [JsonPropertyName("date")]
    public DateOnly Date { get; init; }
    [JsonPropertyName("query")]
    public Query QueryInfo { get; init; }

    [JsonConstructor]
    public ConvertCurrencyData(decimal result, bool success, DateOnly date, Query queryInfo) : this()
    {
        Result = result;
        Success = success;
        Date = date;
        QueryInfo = queryInfo;
    }

    public override readonly string ToString() => $"{QueryInfo.Amount} {QueryInfo.From} = {Result} {QueryInfo.To}";
}
