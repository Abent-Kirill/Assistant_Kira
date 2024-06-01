using Assistant_Kira.DTO;

using System.Collections.Immutable;
using System.Text.Json;

namespace Assistant_Kira;

internal sealed class NewsApiService(IHttpClientFactory httpClientFactory) : INewsApi
{
    public async Task<ImmutableArray<Article>> GetHeadlinesAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("NewsApi");

        var responseGeneral = await httpClient.GetAsync(new Uri("top-headlines?country=ru&category=general&pageSize=5", UriKind.Relative));
        responseGeneral.EnsureSuccessStatusCode();
        var responseScience = await httpClient.GetAsync(new Uri("top-headlines?country=ru&category=science&pageSize=5", UriKind.Relative));
        responseScience.EnsureSuccessStatusCode();
        var responseHealh = await httpClient.GetAsync(new Uri("top-headlines?country=ru&category=health&pageSize=5", UriKind.Relative));
        responseHealh.EnsureSuccessStatusCode();

        var newsGeneral = await JsonSerializer.DeserializeAsync<Articles>(responseGeneral.Content.ReadAsStream());
        var newsScience = await JsonSerializer.DeserializeAsync<Articles>(responseScience.Content.ReadAsStream());
        var newsHealth = await JsonSerializer.DeserializeAsync<Articles>(responseHealh.Content.ReadAsStream());
        newsGeneral.ArticleList.AddRange(newsScience.ArticleList);
        newsGeneral.ArticleList.AddRange(newsHealth.ArticleList);

        return newsGeneral.ArticleList.ToImmutableArray();
    }

    public async Task<ImmutableArray<Article>> GetNewsByAsync(string text)
    {
        using var httpClient = httpClientFactory.CreateClient("NewsApi");

        var response = await httpClient.GetAsync(new Uri($"everything?q={text}&language=ru", UriKind.Relative));
        response.EnsureSuccessStatusCode();

        var news = await JsonSerializer.DeserializeAsync<Articles>(response.Content.ReadAsStream());
        return news.ArticleList.ToImmutableArray();
    }
}
