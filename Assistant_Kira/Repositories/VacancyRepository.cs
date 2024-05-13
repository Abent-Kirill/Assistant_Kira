using System.Collections.Immutable;
using System.Xml.Linq;

using Assistant_Kira.Models;

namespace Assistant_Kira.Repositories;

internal sealed class VacancyRepository(IHttpClientFactory httpClientFactory) : IRepository<Vacancy>
{
    private int _index = 0;
    private ImmutableArray<Vacancy> _vacancies;

    public Vacancy Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _vacancies[_index];
    }

    public Vacancy Next()
    {
        if (_index < _vacancies.Length - 1)
        {
            _index += 1;
        }
        return _vacancies[_index];
    }

    public Vacancy Current()
    {
        LoadVacancyAsync().Wait();
        return _vacancies[_index];
    }

    /// <summary>
    /// Для линивой подгрузки, так как репозиторий singleton, то соостветсвенно при запуске сервера не нужно загружать данные
    /// </summary>
    private async Task LoadVacancyAsync()
    {
        Dispose();

        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(@"https://career.habr.com/vacancies/rss", UriKind.Absolute);

        var response = await httpClient.GetAsync(new Uri(@"?currency=RUR&qid=4&remote=true&skills[]=434&sort=relevance&type=all", UriKind.Relative));
        using var contentStream = await response.Content.ReadAsStreamAsync();
        var xDoc = XDocument.Load(contentStream);
        _vacancies = xDoc.Descendants("item")
                            .Select(item => new Vacancy(
                                item.Element("title")?.Value,
                                item.Element("description")?.Value,
                                item.Element("author")?.Value,
                                new Uri(item.Element("link")?.Value)))
                            .ToImmutableArray();
    }

    public void Dispose()
    {
        _index = 0;
        _vacancies = _vacancies.Clear();
    }
}
