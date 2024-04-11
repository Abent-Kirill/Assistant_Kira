using System.Collections.Immutable;
using System.Xml;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services.NewsServices;

internal sealed class LentaNewsService(IHttpClientFactory httpClientFactory) : INewspaperService
{
    public string Name => "Lenta.ru";

    public async Task<IImmutableList<NewsContent>> GetNewsAsync()
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
}
