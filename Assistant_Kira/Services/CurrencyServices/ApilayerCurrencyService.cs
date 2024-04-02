using System.Text.Json;

using Assistant_Kira.Models.Currencys;

namespace Assistant_Kira.Services.CurrencyServices;

internal sealed class ApilayerCurrencyService(IHttpClientFactory httpClientFactory) : ICurrencyService
{
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="JsonException"></exception>
	public async Task<Currency> GetCurrencyExchangeAsync(string from, string to)
    {
        var httpClient = httpClientFactory.CreateClient("Apilayer");
        var response = await httpClient.GetAsync(new Uri(@$"latest?base={from}&symbols={to}", UriKind.Relative));
        var currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());

        return currencyExchange;
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="JsonException"></exception>
    public async Task<ConvertCurrencyData> CurrencyConversionAsync(int amount, char currencyFrom, char currencyTo)
    {
        var httpClient = httpClientFactory.CreateClient("Apilayer");
        var response = await httpClient.GetAsync(new Uri(@$"convert?to={ICurrencyService.GetCurrencyName(currencyTo)}&from={ICurrencyService.GetCurrencyName(currencyFrom)}&amount={amount}", UriKind.Relative));
        var convertCurrencyData = JsonSerializer.Deserialize<ConvertCurrencyData>(await response.Content.ReadAsStringAsync());

        return convertCurrencyData;
    }
}
