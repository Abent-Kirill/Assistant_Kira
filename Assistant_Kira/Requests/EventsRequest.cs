using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record EventsRequest(DateTimeOffset DateTime) : IRequest<IReadOnlyCollection<string>>
{
}
