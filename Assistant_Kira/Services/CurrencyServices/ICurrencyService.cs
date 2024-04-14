using Assistant_Kira.Models.Currencys;

using Microsoft.OpenApi.Extensions;

namespace Assistant_Kira.Services.CurrencyServices;

internal interface ICurrencyService
{
    public Task<Currency> GetCurrencyExchangeAsync(string from, string to);
    public Task<ConvertCurrencyData> CurrencyConversionAsync(uint amount, string currencyFrom, string currencyTo);

    /// <exception cref="ArgumentException"></exception>
    protected static string GetCurrencyName(char currency) => char.ToLower(currency) switch
    {
        'р' => CurrencyName.RUB.GetDisplayName(),
        'д' => CurrencyName.USD.GetDisplayName(),
        'т' => CurrencyName.KZT.GetDisplayName(),
        'е' => CurrencyName.EUR.GetDisplayName(),
        _ => throw new ArgumentException("Такая валюта не поддерживается")
    };
}
