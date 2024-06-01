using Assistant_Kira.DTO;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record NewsRequest(string Text) : IRequest<Article>
{
}
