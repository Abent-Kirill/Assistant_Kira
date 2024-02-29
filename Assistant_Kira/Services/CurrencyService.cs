using System.Text.Json;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services;

internal sealed partial class CurrencyService(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<Currency> GetCurrencyExchangeAsync(string from, string to)
	{
		var httpClient = _httpClientFactory.CreateClient("Apilayer");

		var response = await httpClient.GetAsync(new Uri(@$"latest?base={from}&symbols={to}", UriKind.Relative));
		var currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());

		return currencyExchange;
	}

	/*
		public async Task<int> CurrencyConversion(int amount, char currencyFrom, char currencyTo)
		{
			var httpClient = _httpClientFactory.CreateClient("Apilayer");
			var response = await httpClient.GetAsync(new Uri(@$"convert?to={GetCurrencyName(currencyTo)}&from={currencyFrom}&amount={amount}", UriKind.Relative));
			var req = JsonSerializer.Deserialize<>(await response.Content.ReadAsStringAsync());
		}*/

	/// <exception cref="ArgumentException"></exception>
	private string GetCurrencyName(char currency) => char.ToLower(currency) switch
	{
		'р' => "RUB",
		'д' => "USD",
		'т' => "KZT",
		_ => throw new ArgumentException("Неизвестный тип")
	};
}
