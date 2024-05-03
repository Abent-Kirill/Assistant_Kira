using Telegram.Bot;

namespace Assistant_Kira.Models;

public sealed class KiraBot : TelegramBotClient
{
    public KiraBot(IConfiguration configuration) : base(configuration["BotSettings:Token"])
    {
        var webhook = configuration["BotSettings:WebhookUrl"];

        ArgumentNullException.ThrowIfNullOrWhiteSpace(webhook, nameof(configuration));

        var hook = new Uri($"{webhook}/api/telegram/update");
        this.SetWebhookAsync(hook.OriginalString).Wait();
    }
}
