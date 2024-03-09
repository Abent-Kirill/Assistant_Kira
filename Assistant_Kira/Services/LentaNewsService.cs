using System.Xml;
using Assistant_Kira.Models;

namespace Assistant_Kira.Services;

internal sealed class LentaNewsService
{
    public LentaNewsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _indexNews = 0;
        _lentaNews = GetLast24().Result;
    }

    IHttpClientFactory _httpClientFactory;
    IEnumerable<LentaNews> _lentaNews;
    int _indexNews;
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
        if (xRoot != null)
        {
            foreach (XmlElement xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    string title = string.Empty, description = string.Empty, newsLink = string.Empty;
                    if (childnode.Name != "item") continue;
                    foreach(XmlNode childnode2 in childnode.ChildNodes)
                    {
                        switch (childnode2.Name)
                        {
                            case "guid":
                                newsLink = childnode2.InnerText;
                                break;
                            case "title":
                                title = childnode2.InnerText;
                                break;
                            case "description":
                                description = childnode2.InnerText;
                                break;
                        }
                    }
                    lentaNews.Add(new LentaNews(new Uri(newsLink), title, description));
                }
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
    public string GetCurrentNews()
    {
        return _lentaNews.ElementAt(_indexNews).ToString();
    }
}
