using System.Text.Json;

using Assistant_Kira.Models.Currencys;

using Microsoft.OpenApi.Extensions;

namespace Assistant_Kira.Services;

internal sealed class ApilayerCurrencyService(IHttpClientFactory httpClientFactory)
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
        var response = await httpClient.GetAsync(new Uri(@$"convert?to={GetCurrencyName(currencyTo)}&from={GetCurrencyName(currencyFrom)}&amount={amount}", UriKind.Relative));
        var convertCurrencyData = JsonSerializer.Deserialize<ConvertCurrencyData>(await response.Content.ReadAsStringAsync());

        return convertCurrencyData;
    }

	/// <exception cref="ArgumentException"></exception>
	private static string GetCurrencyName(char currency) => char.ToLower(currency) switch
	{
		'р' => CurrencyName.RUB.GetDisplayName(),
		'д' => CurrencyName.USD.GetDisplayName(),
		'т' => CurrencyName.KZT.GetDisplayName(),
        'е' => CurrencyName.EUR.GetDisplayName(),
		_ => throw new ArgumentException("Такая валюта не поддерживается")
	};
}
