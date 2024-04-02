using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Services;

public sealed class ServerService(ITelegramBotClient botClient)
{
    public async Task CopyToServer(FileBase fileBase, string path)
    {
        var file = await botClient.GetFileAsync(fileBase.FileId);
        using var stream = new MemoryStream();
        await botClient.DownloadFileAsync(file.FilePath, stream);
        System.IO.File.WriteAllBytes(path, stream.ToArray());
    }
}
