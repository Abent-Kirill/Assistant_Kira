using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class HelloCommand(KiraBot kiraBot) : ICommand
{
    public string Name => "/start";

    public async Task ExecuteAsync(Update update, IEnumerable<string>? arg = null) => await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, $"{GetGreeting()}, {update.Message.From.FirstName}!\nЧто хотите сделать?", replyMarkup: KeyboardPatterns.Menu);
    private static string GetGreeting()
    {
        var currentTime = DateTime.Now;
        var currentHour = currentTime.Hour;

        if (currentHour >= 6 && currentHour < 12)
        {
            return "Доброе утро";
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            return "Добрый день";
        }
        else
        {
            return "Добрый вечер";
        }
    }
}
