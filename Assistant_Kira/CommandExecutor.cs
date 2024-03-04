using System.Text.RegularExpressions;

using Assistant_Kira.Commands;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

internal sealed class CommandExecutor(ILogger<CurrencyCommand> logger, CurrencyService currencyService, IEnumerable<ICommand> commands,  KiraBot kiraBot) : ICommandExecutor
{
    public async Task ExecuteAsync(Update update)
	{
		var text = update.Message!.Text;

		if (string.IsNullOrEmpty(text))
		{
            logger.LogWarning("Ввели пустую команду: {CommandText}", text);
            return;
		}
        
        if (Regex.Match(text, @"\d\s\w\s\w").Success)
        {
            await kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id, commands.Single(x => string.Equals(x.Name, "перевод валют")).Execute(text.Split(' ')));
        }
		try
		{
			var command = commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
			?? throw new ArgumentNullException("Такой команды нет");
			await kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id, command.Execute(text.Split(' ')[1..]));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка команды {text}", text);
			throw;
		}
	}

    public string Execute(string text)
    {
        if (Regex.Match(text, @"\d\s\w\s\w").Success)
        {
            return commands.Single(x => string.Equals(x.Name, "перевод валют")).Execute(text.Split(' '));
        }
        try
        {
            var command = commands.SingleOrDefault(c => c.Name.Equals(text.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Такой команды нет");
            return command.Execute(text.Split(' ')[1..]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка команды {text}", text);
            throw;
        }
    }
}
