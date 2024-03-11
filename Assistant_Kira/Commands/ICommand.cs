using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

public interface ICommand
{
	string Name { get; }
	Task ExecuteAsync(Update update, IEnumerable<string>? arg = null);
}
