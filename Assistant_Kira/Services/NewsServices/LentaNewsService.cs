using System.Collections.Immutable;
using System.Xml;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services.NewsServices;

internal sealed class LentaNewsService : INewspaperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private Timer _timer;
    private ushort _index;
    public IImmutableList<NewsContent> NewsList { get; private set; }
    public LentaNewsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _index = 0;
        NewsList = GetNewsAsync().Result;
        _timer = new Timer(ClearData, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    private void ClearData(object? obj)
    {
        _index = 0;
        NewsList.Clear();
        NewsList = GetNewsAsync().Result;
    }

    public async Task<IImmutableList<NewsContent>> GetNewsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://lenta.ru/rss/", UriKind.Absolute);
        var response = await httpClient.GetAsync(new Uri("last24", UriKind.Relative));
        using var contentStream = await response.Content.ReadAsStreamAsync();
        var lentaNews = new List<NewsContent>();
        var xDoc = new XmlDocument();
        xDoc.Load(contentStream);
        var xRoot = xDoc.DocumentElement;

        if (xRoot is null)
        {
            throw new ArgumentException($"{nameof(xRoot)} был null.\nПараметры:{xDoc}");
        }

        foreach (XmlElement xnode in xRoot)
        {
            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                var title = string.Empty;
                var description = string.Empty;
                var newsLink = string.Empty;

                if (childnode.Name != "item") continue;

                foreach (XmlNode childnodeItem in childnode.ChildNodes)
                {
                    switch (childnodeItem.Name)
                    {
                        case "guid":
                            newsLink = childnodeItem.InnerText;
                            break;
                        case "title":
                            title = childnodeItem.InnerText;
                            break;
                        case "description":
                            description = childnodeItem.InnerText;
                            break;
                    }
                }
                lentaNews.Add(new NewsContent(new Uri(newsLink), title, description));
            }
        }
        return lentaNews.ToImmutableList();
    }

    public NewsContent GetNextNews()
    {
        _index += 1;
        return NewsList[_index];
    }

    public NewsContent GetBackNews()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return NewsList[_index];
    }
}
