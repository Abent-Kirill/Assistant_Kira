using Telegram.Bot;

namespace Assistant_Kira.Models;

public sealed class KiraBot
{
	private readonly IConfiguration _configuration;
	private TelegramBotClient _telegramApi;

	public TelegramBotClient TelegramApi
	{
		get
		{
			if (_telegramApi != null)
			{
				return _telegramApi;
			}

			_telegramApi = new TelegramBotClient(_configuration["BotToken"]);

			var hook = new Uri($"{_configuration["WebhookUrl"]}/api/message/update");
			_telegramApi.SetWebhookAsync(hook.AbsoluteUri).Wait();

			return _telegramApi;
		}
	}

	public KiraBot(IConfiguration configuration) => _configuration = configuration;
}