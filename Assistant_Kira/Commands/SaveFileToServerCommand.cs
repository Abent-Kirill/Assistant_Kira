using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class SaveFileToServerCommand(KiraBot kiraBot, ServerService serverService, IConfiguration configuration, ILogger<SaveFileToServerCommand> logger) : ICommand
{
    public string Name => "Сохранить файл";

    public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        string textMessage;
        try
        {
            var document = update.Message.Document;
            ArgumentNullException.ThrowIfNull(document, nameof(document));
            await serverService.CopyToServer(document, configuration["Paths:Files"]!);
            textMessage = $"Файл {document.FileName} сохранён";
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            textMessage = ex.Message;
        }
        await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, textMessage, replyMarkup: KeyboardPatterns.Menu);
    }
}
