using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class SavePhotoToSeverCommand(KiraBot kiraBot, ServerService serverService, IConfiguration configuration, ILogger<SavePhotoToSeverCommand> logger) : ICommand
{
    public string Name => "сохранить фото";

    public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        string textMessage;
        try
        {
            var photos = update.Message.Photo![^1];
            ArgumentNullException.ThrowIfNull(photos, nameof(photos));
            await serverService.CopyToServer(photos, configuration["Paths:Photos"]);
            textMessage = $"Файл {photos.FileId} сохранён";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            textMessage = ex.Message;
        }
        await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, textMessage, replyMarkup: KeyboardPatterns.Menu);
    }
}
