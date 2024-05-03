using System.Xml;

using Assistant_Kira.Models;

namespace Assistant_Kira.Repositories;

internal sealed class NewsRepository(IHttpClientFactory httpClientFactory) : IRepository<NewsContent>
{
    private uint _index = 0;
    private NewsContent[] _newsContents;

    public NewsContent Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _newsContents[_index];
    }

    public NewsContent Next()
    {
        _index += 1;
        return _newsContents[_index];
    }

    public async Task<IReadOnlyCollection<NewsContent>> GetAllAsync()
    {
        var httpClient = httpClientFactory.CreateClient();
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

                if (childnode.Name != "item")
                {
                    continue;
                }

                foreach (XmlNode childnodeItem in childnode.ChildNodes)
                {
                    switch (childnodeItem.Name)
                    {
                        case "title":
                            title = childnodeItem.InnerText;
                            break;
                        case "description":
                            description = childnodeItem.InnerText;
                            break;
                        case "link":
                            newsLink = childnodeItem.InnerText;
                            break;
                    }
                }
                lentaNews.Add(new NewsContent(new Uri(newsLink), title, description));
            }
        }
        _index = 0;
        _newsContents = [.. lentaNews];
        return lentaNews;
    }
}
