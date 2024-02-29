using Assistant_Kira.Commands;
using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

internal sealed class CommandExecutor(ILogger<CurrencyCommand> logger, IEnumerable<ICommand> commands, KiraBot kiraBot) : ICommandExecutor
{
    public async Task ExecuteAsync(Update update)
	{
		var text = update.Message!.Text;

		if (string.IsNullOrEmpty(text))
		{
            logger.LogWarning("Ввели пустую команду: {CommandText}", text);
            return;
		}
		try
		{
			var command = commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
			?? throw new ArgumentException("Такой команды нет");
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
        try
        {
            var command = commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
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
