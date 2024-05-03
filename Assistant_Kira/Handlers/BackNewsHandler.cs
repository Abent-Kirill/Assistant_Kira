using Assistant_Kira.Models;
using Assistant_Kira.Repositories;

using MediatR;

namespace Assistant_Kira;

internal sealed class BackNewsHandler(IRepository<NewsContent> repository) : IRequestHandler<BackNewsRequest, NewsContent>
{
    public Task<NewsContent> Handle(BackNewsRequest request, CancellationToken cancellationToken) => Task.Run(repository.Back);
}
