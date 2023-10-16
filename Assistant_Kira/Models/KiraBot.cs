using Telegram.Bot;

namespace Assistant_Kira.Models;

public sealed class KiraBot
{
	public TelegramBotClient TelegramApi { get; init; }

	public KiraBot(IConfiguration configuration)
	{
		TelegramApi = new TelegramBotClient(configuration["BotToken"]);
		var hook = $"{configuration["WebhookUrl"]}/api/message/update";
		TelegramApi.SetWebhookAsync(hook).Wait();
	}
}