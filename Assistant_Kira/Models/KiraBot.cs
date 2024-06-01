using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Assistant_Kira.Models;

public sealed class KiraBot : TelegramBotClient, IDisposable
{
    public KiraBot(string token, Uri webHook) : base(token)
    {
        var webhook = new Uri(webHook, "api/telegram/update");
        var all = new List<UpdateType>() { UpdateType.Message, UpdateType.CallbackQuery };
        this.SetWebhookAsync(webhook.ToString(), allowedUpdates: all, maxConnections: 1).Wait();
    }

    public void Dispose()
    {
        this.DeleteWebhookAsync();
    }
}
