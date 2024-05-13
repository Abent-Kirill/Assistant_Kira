using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class NextNewsHandler(IRepository<NewsContent> repository) : IRequestHandler<NextNewsRequest, NewsContent>
{
    public Task<NewsContent> Handle(NextNewsRequest request, CancellationToken cancellationToken) => Task.Run(repository.Next);
}

