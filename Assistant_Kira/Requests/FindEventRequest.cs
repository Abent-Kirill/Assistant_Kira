using Google.Apis.Calendar.v3.Data;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record FindEventRequest(string Text) : IRequest<IReadOnlyCollection<string>>
{
}
