using System.Xml;
using Assistant_Kira.Models;

namespace Assistant_Kira.Services;

internal sealed class LentaNewsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<LentaNews> _lentaNews;
    private int _indexNews;

    public LentaNewsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _lentaNews = GetLast24().Result;
    }

    public async Task<IEnumerable<LentaNews>> GetLast24()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://lenta.ru/rss/", UriKind.Absolute);
        var response = await httpClient.GetAsync(new Uri("last24", UriKind.Relative));
        var contentStream = await response.Content.ReadAsStreamAsync();
        var lentaNews = new List<LentaNews>();
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
                lentaNews.Add(new LentaNews(new Uri(newsLink), title, description));
            }
        }
        return lentaNews;
    }

    public bool NextNews()
    {
        if(_indexNews == _lentaNews.Count() - 1) return false;
        _indexNews++;
        return true;
    }
    public bool PreviousNews()
    {
        if(_indexNews == 0) return false;
        _indexNews--;
        return true;
}
    public LentaNews GetCurrentNews() => _lentaNews.ElementAt(_indexNews);
}
