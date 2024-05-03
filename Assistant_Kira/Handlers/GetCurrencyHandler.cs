using System.Collections.ObjectModel;
using System.Text.Json;

using Assistant_Kira.DTO.Currencys;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class GetCurrencyHandler(IHttpClientFactory httpClientFactory) : IRequestHandler<GetCurrencyRequest, IReadOnlyCollection<Currency>>
{
    private readonly ReadOnlyDictionary<string, string> _currentCurrencyNameArray = new(new Dictionary<string, string>()
    {
            {"USD", "RUB"},
            {"EUR", "RUB"},
            {"RUB", "KZT"}
    });

    public async Task<IReadOnlyCollection<Currency>> Handle(GetCurrencyRequest request, CancellationToken cancellationToken)
    {
        var currencyList = new List<Currency>();

        foreach (var currency in _currentCurrencyNameArray)
        {
            currencyList.Add(await GetCurrencyExchangeAsync(currency.Key, currency.Value));
        }
        
        return currencyList;
    }

    private async Task<Currency> GetCurrencyExchangeAsync(string from, string to)
    {
        var httpClient = httpClientFactory.CreateClient("Apilayer");
        var response = await httpClient.GetAsync(new Uri(@$"latest?base={from}&symbols={to}", UriKind.Relative));
        var currencyExchange = JsonSerializer.Deserialize<Currency>(await response.Content.ReadAsStringAsync());

        return currencyExchange;
    }
}
