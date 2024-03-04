using Telegram.Bot;

namespace Assistant_Kira.Models;

public sealed class KiraBot
{
	public TelegramBotClient TelegramApi { get; init; }

	public KiraBot(IConfiguration configuration)
	{
        var botToken = configuration["BotToken"];

        if(string.IsNullOrWhiteSpace(botToken))
        {
            throw new ArgumentNullException(botToken, "Bot Token пуст");
        }

        TelegramApi = new TelegramBotClient(botToken);
        var webhook = configuration["WebhookUrl"];

        if (string.IsNullOrWhiteSpace(webhook))
        {
            throw new ArgumentNullException(webhook, "Webhook url пуст");
        }

		var hook = new Uri($"{webhook}/api/message/update");
		TelegramApi.SetWebhookAsync(hook.OriginalString).Wait();
	}
}
