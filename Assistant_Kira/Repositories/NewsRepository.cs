using System.Collections.Immutable;
using System.Text.Json;

using Assistant_Kira.DTO;

namespace Assistant_Kira.Repositories;

internal sealed class NewsRepository(IHttpClientFactory httpClientFactory) : IRepository<Article>
{
    private int _index = 0;
    private ImmutableArray<Article> _newsContents;

    public Article Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _newsContents.ElementAt(_index);
    }

    public Article Next()
    {
        if (_index < _newsContents.Length - 1)
        {
            _index += 1;
        }
        return _newsContents.ElementAt(_index);
    }

    public Article Current()
    {
        LoadNewsAsync().Wait();
        return _newsContents.ElementAt(_index);
    }

    /// <summary>
    /// Для линивой подгрузки, так как репозиторий singleton, то соостветсвенно при запуске сервера не нужно загружать данные
    /// </summary>
    private async Task LoadNewsAsync()
    {
        Dispose();
        _newsContents = await GetHeadlinesFromNewsApi();
    }

  /*  private async Task<ImmutableArray<NewsContent>> GetNewsFromLentaAsync()
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://lenta.ru/rss/", UriKind.Absolute);
        var response = await httpClient.GetAsync(new Uri("last24", UriKind.Relative));
        response.EnsureSuccessStatusCode();

        using var contentStream = await response.Content.ReadAsStreamAsync();
        var xDoc = XDocument.Load(contentStream);
        return xDoc.Descendants("item")
                            .Select(item => new NewsContent(
                                new Uri(item.Element("link")?.Value),
                                item.Element("title")?.Value,
                                item.Element("description")?.Value))
                            .ToImmutableArray();
    }
  */

    //TODO: Перенести в отдельный сервис
    private async Task<ImmutableArray<Article>> GetHeadlinesFromNewsApi()
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://newsapi.org/v2/", UriKind.Absolute);
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
        httpClient.DefaultRequestHeaders.Add("x-api-key", "builder.Configuration[NewsApiKey]");

        var response = await httpClient.GetAsync(new Uri("top-headlines?country=us&pageSize=20", UriKind.Relative));
        response.EnsureSuccessStatusCode();

        var news = await JsonSerializer.DeserializeAsync<Articles>(response.Content.ReadAsStream());
        return news.ArticleList.ToImmutableArray();
    }

    public void Dispose()
    {
        _index = 0;
        _newsContents = _newsContents.Clear();
    }
}
