using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Assistant_Kira.Models;

public sealed class KiraBot : TelegramBotClient, IAsyncDisposable
{
    private KiraBot(string token) : base(token) { }

    public static async Task<KiraBot> CreateAsync(string token, Uri webHook)
    {
        var bot = new KiraBot(token);
        var webhook = new Uri(webHook, "api/telegram/update");
        await bot.SetWebhookAsync(webhook.ToString(),
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
            maxConnections: 1);
        return bot;
    }

    public async ValueTask DisposeAsync() => await this.DeleteWebhookAsync();
}
