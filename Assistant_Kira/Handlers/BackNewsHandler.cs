using Assistant_Kira.DTO;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class BackNewsHandler(IRepository<Article> repository) : IRequestHandler<BackNewsRequest, Article>
{
    public Task<Article> Handle(BackNewsRequest request, CancellationToken cancellationToken) => Task.Run(repository.Back);
}
