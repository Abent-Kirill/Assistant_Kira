using System.Text.RegularExpressions;

using Assistant_Kira.Commands;
using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

internal sealed partial class CommandExecutor(ILogger<CurrencyCommand> logger, IEnumerable<ICommand> commands, KiraBot kiraBot)
    : ICommandExecutor
{
    public async Task ExecuteAsync(Update update)
    {
        var text = update.Message!.Text;

        if (string.IsNullOrEmpty(text))
        {
            logger.LogWarning("Ввели пустую команду: {CommandText}", text);
            return;
        }

        if (ConvertCurrencyRegex().Match(text).Success)
        {
            await kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id,
                await commands.Single(x => string.Equals(x.Name, "перевод валют"))
                .ExecuteAsync(text.Split(' ')));
        }

        var command = commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException("Такой команды нет");
        await kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id, await command.ExecuteAsync(text.Split(' ')[1..]));
    }

    public async Task<string> ExecuteAsync(string text)
    {
        if (ConvertCurrencyRegex().Match(text).Success)
        {
            return await commands.Single(x => string.Equals(x.Name, "перевод валют")).ExecuteAsync(text.Split(' '));
        }

        var command = commands.SingleOrDefault(c => c.Name.Equals(text.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException("Такой команды нет");
        return await command.ExecuteAsync(text.Split(' ')[1..]);
    }

    [GeneratedRegex(@"\d\s\w\s\w")]
    private static partial Regex ConvertCurrencyRegex();
}
