using System.Xml;

using Assistant_Kira.Models;

namespace Assistant_Kira.Repositories;

internal sealed class VacancyRepository(IHttpClientFactory httpClientFactory) : IRepository<Vacancy>
{
    private uint _index = 0;
    private Vacancy[] _vacancies;

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
        _index += 1;
        return _vacancies[_index];
    }

    public async Task<IReadOnlyCollection<Vacancy>> GetAllAsync()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(@"https://career.habr.com/vacancies/rss", UriKind.Absolute);

        var response = await httpClient.GetAsync(new Uri(@"?currency=RUR&qid=4&remote=true&skills[]=434&sort=relevance&type=all", UriKind.Relative));
        using var contentStream = await response.Content.ReadAsStreamAsync();
        var vacancies = new List<Vacancy>();
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
                var link = string.Empty;
                var companyName = string.Empty;

                if (childnode.Name != "item")
                {
                    continue;
                }

                foreach (XmlNode childnodeItem in childnode.ChildNodes)
                {
                    switch (childnodeItem.Name)
                    {
                        case "link":
                            link = childnodeItem.InnerText;
                            break;
                        case "title":
                            title = childnodeItem.InnerText;
                            break;
                        case "description":
                            description = childnodeItem.InnerText;
                            break;
                        case "author":
                            companyName = childnodeItem.InnerText;
                            break;
                    }
                }
                vacancies.Add(new Vacancy(title, description, companyName, new Uri(link)));
            }
        }
        _index = 0;
        _vacancies = vacancies.ToArray();
        return vacancies;
    }
}
