using System.Text.RegularExpressions;

using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Assistant_Kira;

internal sealed partial class CommandExecutor(ILogger<CurrencyCommand> logger, IEnumerable<ICommand> commands,
    KiraBot kiraBot, ServerService serverService, IConfiguration configuration): ICommandExecutor
{
    public async Task ExecuteAsync(Update update)
    {
        ArgumentNullException.ThrowIfNull(update, nameof(update));
       
        var chatId = update.Message.Chat.Id;
        switch (update.Message.Type)
        {
            case MessageType.Text:
                var text = update.Message!.Text;
                if (string.IsNullOrEmpty(text))
                {
                    logger.LogWarning("Ввели пустую команду: {CommandText}", text);
                    return;
                }
                if (ConvertCurrencyRegex().Match(text).Success)
                {
                    await kiraBot.SendTextMessageAsync(chatId,
                        await commands.Single(x => x.Name.Equals("перевод валют", StringComparison.OrdinalIgnoreCase))
                        .ExecuteAsync(text.Split(' ')), replyMarkup: KeyboardPatterns.Menu);
                    return;
                }

                var command = commands.SingleOrDefault(c => c.Name.Equals(text.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Такой команды нет");
                await kiraBot.SendTextMessageAsync(chatId, await command.ExecuteAsync(text.Split(' ')[1..]));
                break;
            case MessageType.Photo:
                var photos = update.Message.Photo[^1];
                await serverService.CopyToServer(photos, configuration["Paths:Photos"]);
                break;
            case MessageType.Audio:
                await kiraBot.SendTextMessageAsync(chatId, "Ещё не реализовано действие", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Video:
                await kiraBot.SendTextMessageAsync(chatId, "Ещё не реализовано действие", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Voice:
                await kiraBot.SendTextMessageAsync(chatId, "Ещё не реализовано действие", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Document:
                var document = update.Message.Document;
                ArgumentNullException.ThrowIfNull(document, nameof(document));
                await serverService.CopyToServer(document, configuration["Paths:Files"]);
                await kiraBot.SendTextMessageAsync(chatId, "Сохранено", replyMarkup: KeyboardPatterns.TestInlineKeyboard) ;
                break;
            case MessageType.Location:
                await kiraBot.SendTextMessageAsync(chatId, "Ещё не реализовано действие", replyMarkup: KeyboardPatterns.Menu);
                break;
            default:
                await kiraBot.SendTextMessageAsync(chatId, "Такой команды нет", replyMarkup: KeyboardPatterns.Menu);
                break;
        }
    }

    public async Task<string> ExecuteAsync(string text)
    {
        if (ConvertCurrencyRegex().Match(text).Success)
        {
            return await commands.Single(x => string.Equals(x.Name, "перевод валют"))
                .ExecuteAsync(text.Split(' '));
        }

        var command = commands.SingleOrDefault(c => c.Name.Equals(text.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException("Такой команды нет");
        return await command.ExecuteAsync(text.Split(' ')[1..]);
    }

    [GeneratedRegex(@"\d\s\w\s\w")]
    private static partial Regex ConvertCurrencyRegex();
}
