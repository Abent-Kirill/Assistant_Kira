using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record GetListEventsRequest(double Days) : IRequest<IReadOnlyCollection<string>>
{
}
