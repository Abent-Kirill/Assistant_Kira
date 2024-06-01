using System.Collections.Immutable;

using Assistant_Kira.DTO;

namespace Assistant_Kira.Repositories;

internal sealed class NewsRepository : IRepository<Article>
{
    private int _index = 0;
    public ImmutableArray<Article> Contents { get; set; }

    public Article Back()
    {
        if (_index > 0)
        {
            _index -= 1;
        }

        return Contents.ElementAt(_index);
    }

    public Article Next()
    {
        if (_index < Contents.Length - 1)
        {
            _index += 1;
        }
        return Contents.ElementAt(_index);
    }

    public Article Current()
    {
        return Contents.ElementAt(_index);
    }

    public void Dispose()
    {
        _index = 0;
        Contents = Contents.Clear();
    }
}
