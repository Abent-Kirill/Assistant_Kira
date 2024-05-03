using System.Text.Json;

using Assistant_Kira.DTO.Currencys;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class ConvertCurrencyHandler(IHttpClientFactory httpClientFactory) : IRequestHandler<ConvertCurrencyRequest, ConvertCurrencyData>
{
    public async Task<ConvertCurrencyData> Handle(ConvertCurrencyRequest request, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient("Apilayer");
        var response = await httpClient.GetAsync(new Uri(@$"convert?to={request.CurrencyTo}&from={request.CurrencyFrom}&amount={request.Amount}", UriKind.Relative), cancellationToken);
        var convertCurrencyData = JsonSerializer.Deserialize<ConvertCurrencyData>(await response.Content.ReadAsStringAsync(cancellationToken));

        return convertCurrencyData;
    }
}
