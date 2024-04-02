using Assistant_Kira.Services.NewsServices;

namespace Assistant_Kira.Commands;

internal sealed class NewsCommand(INewspaperService newsService, ILogger<NewsCommand> logger) : Command
{
    public override string Name => "Новости";

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        string textMessage;

        if(args != null && args.Length > 0)
        {
            if (args[0].Equals("вперёд", StringComparison.CurrentCultureIgnoreCase))
            {
                return newsService.GetNextNews().ToString();
            }
            else if (args[0].Equals("назад", StringComparison.CurrentCultureIgnoreCase))
            {
                return newsService.GetBackNews().ToString();
            }
        }
        try
        {
            var news = await newsService.GetNewsAsync();
            textMessage = news[0].ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {args}", args);
            textMessage = "Не удалось получить новости из Lenta. Попробуйте позднее.";
        }

       return textMessage;
    }
}
