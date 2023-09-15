using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

public sealed class HelloCommand : ICommand
{
    private readonly TelegramBotClient _kiraBot;

    public HelloCommand(TelegramBotClient kiraBot) => _kiraBot = kiraBot;

    public async Task ExecuteAsync(Update update) => await _kiraBot.SendTextMessageAsync(update.Message!.Chat.Id, $"Приветствую, {update.Message.Chat.FirstName}!");
}
