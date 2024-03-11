using Assistant_Kira.Models;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Services;

internal sealed class ServerService(KiraBot kiraBot)
{
    public async Task CopyToServer(FileBase fileBase, string path)
    {
        var file = await kiraBot.GetFileAsync(fileBase.FileId);
        using var stream = new MemoryStream();
        await kiraBot.DownloadFileAsync(file.FilePath, stream);
        System.IO.File.WriteAllBytes(path, stream.ToArray());
    }
}
