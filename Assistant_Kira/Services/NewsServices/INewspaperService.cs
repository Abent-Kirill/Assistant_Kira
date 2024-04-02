using System.Collections.Immutable;

using Assistant_Kira.Models;

namespace Assistant_Kira.Services.NewsServices;

internal interface INewspaperService
{
    public Task<IImmutableList<NewsContent>> GetNewsAsync();
    public NewsContent GetNextNews();
    public NewsContent GetBackNews();
}
