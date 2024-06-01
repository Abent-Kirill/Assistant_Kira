using Assistant_Kira.DTO;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class NextNewsHandler(IRepository<Article> repository) : IRequestHandler<NextNewsRequest, Article>
{
    public Task<Article> Handle(NextNewsRequest request, CancellationToken cancellationToken) => Task.Run(repository.Next);
}

