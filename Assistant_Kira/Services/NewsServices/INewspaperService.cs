using System.Collections.Immutable;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services.NewsServices;

internal interface INewspaperService
{
    public string Name { get; }
    public Task<IImmutableList<NewsContent>> GetNewsAsync();
}
