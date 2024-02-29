using Telegram.Bot.Types;

namespace Assistant_Kira;

public interface ICommandExecutor
{
	Task ExecuteAsync(Update update);
    string Execute(string text);
}
