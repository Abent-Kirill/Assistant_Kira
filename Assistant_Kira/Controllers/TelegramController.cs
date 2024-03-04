using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/telegram/update")]
public sealed class TelegramController(ICommandExecutor commandExecutor, ILogger<TelegramController> logger) : ControllerBase
{
    [HttpPost]
    public async Task Update([FromBody] object updateObj)
	{
		//var options = new JsonSerializerOptions()
		//{
		//	PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		//	PropertyNameCaseInsensitive = true
		//};
		//options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		//options.Converters.Add(new UnixTimestampConverter());

		try
		{
            ArgumentNullException.ThrowIfNull(updateObj);

            var update = JsonSerializer.Deserialize<Update>(updateObj.ToString()!);
			if (update is not null & update!.Message is not null)
			{
				await commandExecutor.ExecuteAsync(update);
			}
		}
		catch (Exception ex)
		{
            logger.LogError(ex, "Ошибка при выполнении команды");
		}
	}
}
