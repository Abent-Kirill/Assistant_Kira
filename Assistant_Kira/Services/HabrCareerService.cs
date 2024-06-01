using Assistant_Kira.Models;

using System.Collections.Immutable;
using System.Xml.Linq;

namespace Assistant_Kira.Services;

internal sealed class HabrCareerService(IHttpClientFactory httpClientFactory)
{
    public async Task<ImmutableArray<Vacancy>> GetVacancies()
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://career.habr.com/vacancies/rss", UriKind.Absolute);

        var response = await httpClient.GetAsync(new Uri("?currency=RUR&qid=4&remote=true&skills[]=434&sort=relevance&type=all", UriKind.Relative));
        using var contentStream = await response.Content.ReadAsStreamAsync();
        var xDoc = XDocument.Load(contentStream);
        return xDoc.Descendants("item")
                            .Select(item => new Vacancy(
                                item.Element("title")?.Value,
                                item.Element("description")?.Value,
                                item.Element("author")?.Value,
                                new Uri(item.Element("link")?.Value)))
                            .ToImmutableArray();
    }
}
