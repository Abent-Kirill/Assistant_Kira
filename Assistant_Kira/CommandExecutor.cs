using Assistant_Kira.Commands;
using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

internal sealed class CommandExecutor : ICommandExecutor
{
	private readonly ILogger<CurrencyCommand> _logger;
	private readonly IEnumerable<ICommand> _commands;
	private readonly KiraBot _kiraBot;

	public CommandExecutor(ILogger<CurrencyCommand> logger, IEnumerable<ICommand> commands, KiraBot kiraBot)
	{
		_logger = logger;
		_commands = commands;
		_kiraBot = kiraBot;
	}

	public async Task ExecuteAsync(Update update)
	{
		var text = update.Message!.Text;

		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		try
		{
			var command = _commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
			?? throw new ArgumentException("Такой команды нет");
			await _kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id, command.Execute(text.Split(' ')[1..]));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка команды {text}", text);
			throw;
		}
	}

    public string Execute(string text)
    {
        try
        {
            var command = _commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Такой команды нет");
            return command.Execute(text.Split(' ')[1..]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка команды {text}", text);
            throw;
        }
    }
}
