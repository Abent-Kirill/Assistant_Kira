using Assistant_Kira.Commands;
using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

internal sealed class CommandExecutor : ICommandExecutor
{
	private readonly IEnumerable<ICommand> _commands;
	private readonly KiraBot _kiraBot;

	public CommandExecutor(IEnumerable<ICommand> commands, KiraBot kiraBot)
	{
		_commands = commands;
		_kiraBot = kiraBot;
	}

	public async Task ExecuteAsync(Update update)
	{
		var text = update.Message?.Text;

		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		var command = _commands.SingleOrDefault(c => c.Name.Equals(text, StringComparison.OrdinalIgnoreCase));

		if (command != null)
		{
			await _kiraBot.TelegramApi.SendTextMessageAsync(update.Message.Chat.Id, command.Execute());
		}
		else
		{
			throw new ArgumentException("Такой команды нет");
			//await HandleUnknownCommand(update);
		}
	}
}
