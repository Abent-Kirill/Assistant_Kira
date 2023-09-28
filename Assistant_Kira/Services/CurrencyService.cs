using System.Text.Json;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services;

internal sealed class CurrencyService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConfiguration _configuration;

	public CurrencyService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		_httpClientFactory = httpClientFactory;
		_configuration = configuration;
	}
	public async Task<List<Currency>> GetCurrencyExchangeAsync()
	{
		var httpClient = _httpClientFactory.CreateClient();
		httpClient.BaseAddress = new Uri(@"https://api.apilayer.com/fixer/", UriKind.Absolute);
		httpClient.DefaultRequestHeaders.Add("apikey", _configuration["ApikeyCurrency"]);
		var valutes = new string[] { "USD", "EUR" };

		var currencyList = new List<Currency>(4);
		var response = await httpClient.GetAsync(new Uri(@$"latest?base=RUB&symbols=KZT", UriKind.Relative));
		var currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());
		currencyExchange.Name = "KZT";
		currencyList.Add(currencyExchange);

		foreach (var valute in valutes)
		{
			response = await httpClient.GetAsync(new Uri(@$"latest?base={valute}&symbols=RUB", UriKind.Relative));
			currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());
			currencyList.Add(currencyExchange);
		}

		return currencyList;
	}
}