using Assistant_Kira.DTO;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record BackNewsRequest : IRequest<Article>
{
}
