using Assistant_Kira.DTO;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record NextNewsRequest : IRequest<Article>
{
}
