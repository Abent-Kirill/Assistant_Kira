using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira;

internal sealed class NewsHandler(IRepository<NewsContent> repository) : IRequestHandler<NewsRequest, NewsContent>
{
    public Task<NewsContent> Handle(NewsRequest request, CancellationToken cancellationToken) => Task.Run(repository.Current);
}
