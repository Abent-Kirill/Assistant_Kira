using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace Assistant_Kira.Models;

public class KiraBot
{
	private IConfiguration _configuration;
	private BotClient _botClient;

	public KiraBot(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<BotClient> GetBot()
	{
		if (_botClient != null)
		{
			return _botClient;
		}

		_botClient = new BotClient(_configuration["Token"]);

		var hook = $"{_configuration["Url"]}api/message/update";
		await _botClient.SetWebhookAsync(hook);

		return _botClient;
	}
}