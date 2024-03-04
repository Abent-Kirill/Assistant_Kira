using Telegram.Bot;

namespace Assistant_Kira.Models;

//TODO: Унаследоваться от TelegramBotClient?
public sealed class KiraBot
{
	public TelegramBotClient TelegramApi { get; init; }

	public KiraBot(IConfiguration configuration)
	{
        var botToken = configuration["BotToken"];

        ArgumentNullException.ThrowIfNullOrWhiteSpace(botToken, nameof(configuration));

        TelegramApi = new TelegramBotClient(botToken);
        var webhook = configuration["WebhookUrl"];

        ArgumentNullException.ThrowIfNullOrWhiteSpace(webhook, nameof(configuration));

		var hook = new Uri($"{webhook}/api/telegram/update");
		TelegramApi.SetWebhookAsync(hook.OriginalString).Wait();
	}
}
