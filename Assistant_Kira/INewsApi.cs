using Assistant_Kira.DTO;
using System.Collections.Immutable;

namespace Assistant_Kira;

internal interface INewsApi
{
    public Task<ImmutableArray<Article>> GetHeadlinesAsync();
    public Task<ImmutableArray<Article>> GetNewsByAsync(string text);
}
