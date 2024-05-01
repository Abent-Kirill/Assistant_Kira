using Assistant_Kira.Commands;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Assistant_Kira;

public sealed class CallbackExecutor(IEnumerable<Command> commands, ITelegramBotClient botClient, IConfiguration configuration)
{
    public async Task<Message> Execute(long chatId, int messageId, string data)
    {
        if (chatId != Convert.ToInt64(configuration["BotSettings:ChatId"]))
        {
            throw new UnauthorizedAccessException("Вы не являетесь человеком с которым я работаю. Всего хорошего");
        }

        string command = string.Empty;
        string arg = string.Empty;
        InlineKeyboardMarkup? replyMarkup = InlineKeyboardMarkup.Empty();

        switch (data)
        {
            case "Вперёд":
                command = "Новости";
                arg = "Вперёд";
                replyMarkup = KeyboardSamples.NewsKeyboard;
                break;
            case "Назад":
                command = "Новости";
                arg = "Назад";
                replyMarkup = KeyboardSamples.NewsKeyboard;
                break;
            case "Вперёд v":
                command = "Вакансии";
                arg = "Вперёд";
                replyMarkup = KeyboardSamples.VacanciesKeyboard;
                break;
            case "Назад v":
                command = "Вакансии";
                arg = "Назад";
                replyMarkup = KeyboardSamples.VacanciesKeyboard;
                break;
        }

        Command currentCommand = commands.Single(x => x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
        var result = await currentCommand.ExecuteAsync(arg);
        return await botClient.EditMessageTextAsync(chatId, messageId, result, replyMarkup: replyMarkup);
    }
}
