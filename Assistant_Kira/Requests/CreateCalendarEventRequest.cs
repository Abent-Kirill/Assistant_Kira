using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record CreateCalendarEventRequest(string Text) : IRequest<bool>
{
}
