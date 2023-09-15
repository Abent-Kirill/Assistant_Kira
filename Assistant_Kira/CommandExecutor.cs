using Assistant_Kira.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira;

public sealed class CommandExecutor
{
	private ICommand _command;
	private readonly TelegramBotClient _botClient;

	public CommandExecutor(TelegramBotClient botClient) => _botClient = botClient;

	public async Task ExecuteAsync(Update update)
	{
		switch (update.Message!.Text!.ToLower())
		{
			case "/start":
				_command = new HelloCommand(_botClient);
				await _command.ExecuteAsync(update);
				break;
			case "документ":
				_command = new WeatherCommand(_botClient);
				break;
			default:
				break;
		}
	}
}
