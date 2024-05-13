using System.Collections.Immutable;
using System.Xml.Linq;

using Assistant_Kira.Models;

namespace Assistant_Kira.Repositories;

internal sealed class NewsRepository(IHttpClientFactory httpClientFactory) : IRepository<NewsContent>
{
    private int _index = 0;
    private ImmutableArray<NewsContent> _newsContents;

    public NewsContent Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _newsContents.ElementAtOrDefault(_index);
    }

    public NewsContent Next()
    {
        if (_index < _newsContents.Length - 1)
        {
            _index += 1;
        }
        return _newsContents.ElementAtOrDefault(_index);
    }

    public NewsContent Current()
    {
        LoadNewsAsync().Wait();
        return _newsContents.ElementAtOrDefault(_index);
    }

    /// <summary>
    /// Для линивой подгрузки, так как репозиторий singleton, то соостветсвенно при запуске сервера не нужно загружать данные
    /// </summary>
    private async Task LoadNewsAsync()
    {
        Dispose();
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://lenta.ru/rss/", UriKind.Absolute);
        var response = await httpClient.GetAsync(new Uri("last24", UriKind.Relative));
        using var contentStream = await response.Content.ReadAsStreamAsync();
        var xDoc = XDocument.Load(contentStream);
        _newsContents = xDoc.Descendants("item")
                            .Select(item => new NewsContent(
                                new Uri(item.Element("link")?.Value),
                                item.Element("title")?.Value,
                                item.Element("description")?.Value))
                            .ToImmutableArray();
    }

    public void Dispose()
    {
        _index = 0;
        _newsContents = _newsContents.Clear();
    }
}
