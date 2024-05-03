using Assistant_Kira.DTO.Currencys;

using MediatR;

namespace Assistant_Kira.Requests;

internal record GetCurrencyRequest() : IRequest<IReadOnlyCollection<Currency>>
{
}
