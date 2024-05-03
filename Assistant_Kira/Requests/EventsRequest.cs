using MediatR;

namespace Assistant_Kira;

internal sealed record EventsRequest(DateTimeOffset DateTime) : IRequest<IReadOnlyCollection<string>>
{
}
