using Assistant_Kira.DTO.Currencys;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record ConvertCurrencyRequest(uint Amount, string CurrencyFrom, string CurrencyTo) : IRequest<ConvertCurrencyData>
{
}