using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

public sealed class WeatherCommand : ICommand
{
	private readonly TelegramBotClient _botClient;

	public WeatherCommand(TelegramBotClient botClient) => _botClient = botClient;

	public async Task ExecuteAsync(Update update) => await _botClient.SendTextMessageAsync(update.Message!.Chat.Id, "Скоро буит!");
}
