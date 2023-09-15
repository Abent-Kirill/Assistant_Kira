using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

public interface ICommand
{
	Task ExecuteAsync(Update update);
}
