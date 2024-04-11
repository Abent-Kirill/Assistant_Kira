using Assistant_Kira.Models;
using System.Collections.Immutable;

using Assistant_Kira.Services.NewsServices;

namespace Assistant_Kira.Commands;

internal sealed class NewsCommand(INewspaperService newsService, ILogger<NewsCommand> logger) : Command
{
    private ushort _index;
    private IImmutableList<NewsContent> _newsList;

    public override string Name => "Новости";

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        string textMessage;

        if(args != null && args.Length > 0)
        {
            if (args[0].Equals("вперёд", StringComparison.CurrentCultureIgnoreCase))
            {
                return GetNextNews().ToString();
            }
            else if (args[0].Equals("назад", StringComparison.CurrentCultureIgnoreCase))
            {
                return GetBackNews().ToString();
            }
        }
        try
        {
            _newsList = await newsService.GetNewsAsync();
            textMessage = _newsList[0].ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {args}", args);
            textMessage = $"Не удалось получить новости из {newsService.Name}. Попробуйте позднее.";
        }

       return textMessage;
    }

    private NewsContent GetNextNews()
    {
        _index += 1;
        return _newsList[_index];
    }

    private NewsContent GetBackNews()
    {
        if (_index > 0)
        {
            _index -= 1;
        }
        return _newsList[_index];
    }
}
