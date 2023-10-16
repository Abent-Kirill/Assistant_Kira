using System.Text.Json;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services;

internal sealed partial class CurrencyService(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<List<Currency>> GetCurrencyExchangeAsync()
	{
		var httpClient = _httpClientFactory.CreateClient("Apilayer");
		var currencyNameArray = new string[] { "USD", "EUR" };

		var currencyExchanges = new List<Currency>(4);
		var response = await httpClient.GetAsync(new Uri(@$"latest?base=RUB&symbols=KZT", UriKind.Relative));
		var currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());
		currencyExchange.Name = "KZT";
		currencyExchanges.Add(currencyExchange);

		foreach (var currencyName in currencyNameArray)
		{
			response = await httpClient.GetAsync(new Uri(@$"latest?base={currencyName}&symbols=RUB", UriKind.Relative));
			currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());
			currencyExchanges.Add(currencyExchange);
		}

		return currencyExchanges;
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
		_ => throw new ArgumentException("Не известный тип")
	};
}