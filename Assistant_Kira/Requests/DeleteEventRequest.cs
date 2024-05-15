using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record DeleteEventRequest(string EventId) : IRequest<string>
{
}
