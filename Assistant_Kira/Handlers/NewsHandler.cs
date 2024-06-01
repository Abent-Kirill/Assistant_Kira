using Assistant_Kira.DTO;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class NewsHandler(INewsApi newsApi, IRepository<Article> repository) : IRequestHandler<NewsRequest, Article>
{
    public async Task<Article> Handle(NewsRequest request, CancellationToken cancellationToken)
    {
        repository.Dispose();
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            repository.Contents = await newsApi.GetHeadlinesAsync();
        }
        else
        {
            repository.Contents = await newsApi.GetNewsByAsync(request.Text);
        }

        return repository.Current();
    }
}
