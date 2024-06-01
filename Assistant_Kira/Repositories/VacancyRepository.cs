using System.Collections.Immutable;

using Assistant_Kira.Models;

namespace Assistant_Kira.Repositories;

internal sealed class VacancyRepository : IRepository<Vacancy>
{
    private int _index = 0;
    public ImmutableArray<Vacancy> Contents { get; set; }

    public Vacancy Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return Contents[_index];
    }

    public Vacancy Next()
    {
        if (_index < Contents.Length - 1)
        {
            _index += 1;
        }
        return Contents[_index];
    }

    public Vacancy Current()
    {
        return Contents[_index];
    }

    public void Dispose()
    {
        _index = 0;
        Contents = Contents.Clear();
    }
}
