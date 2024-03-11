using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class NewsCommand(LentaNewsService lentaNewsService, KiraBot kiraBot, ILogger<NewsCommand> logger) : ICommand
{
    public string Name => "Новости";

    public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        string textMessage;
        try
        {
            textMessage = lentaNewsService.GetCurrentNews().ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            textMessage = "Не удалось получить новости из Lenta. Попробуйте позднее.";
        }

        await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, textMessage, replyMarkup: KeyboardPatterns.TestInlineKeyboard);
    }
}
