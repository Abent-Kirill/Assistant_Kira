using System.Collections.Immutable;

using Assistant_Kira.Models;
using Assistant_Kira.Services.NewsServices;

namespace Assistant_Kira.Commands;

internal sealed class HabrVacanciesCommand(HabrCareerService habrCareerService) : Command
{
    private ushort _index;
    private IImmutableList<Vacancy> _vacancies;

    public override string Name => "Вакансии";

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        string textMessage;

        if (args != null && args.Length > 0)
        {
            if (args[0].Equals("вперёд", StringComparison.CurrentCultureIgnoreCase))
            {
                var nextVacancy = GetNextVacancy();
                return $"Страница: {_index+1}/{_vacancies.Count-1}\n{nextVacancy}";
            }
            else if (args[0].Equals("назад", StringComparison.CurrentCultureIgnoreCase))
            {
                var backVacancy = GetBackVacancy();
                return $"Страница: {_index+1}/{_vacancies.Count-1}\n{backVacancy}";
            }
        }
        try
        {
            _vacancies = await habrCareerService.GetVacanciesAsync();
            _index = 0;
            textMessage = $"Страница: {_index+1}/{_vacancies.Count-1}\n{_vacancies[0]}";
        }
        catch (Exception)
        {
            textMessage = "Не удалось получить новости из источника. Попробуйте позднее.";
        }

        return textMessage;
    }

    private Vacancy GetNextVacancy()
    {
        _index += 1;
        return _vacancies[_index];
    }

    private Vacancy GetBackVacancy()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _vacancies[_index];
    }
}
