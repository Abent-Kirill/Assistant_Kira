using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/message/update")]
public sealed class KiraBotController : ControllerBase
{
	private readonly ICommandExecutor _commandExecutor;

	public KiraBotController(ICommandExecutor commandExecutor)
	{
		_commandExecutor = commandExecutor;
	}

	[HttpPost]
	public async Task<IActionResult> Update([FromBody] object updateObj)
	{

		var options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true
		};
		options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		options.Converters.Add(new UnixTimestampConverter());
		try
		{
			if (updateObj is null)
			{
				return BadRequest("Пустой запрос");
			}

			var update = JsonSerializer.Deserialize<Update>(updateObj.ToString()!, options);
			if (update is not null & update!.Message is not null)
			{
				await _commandExecutor.ExecuteAsync(update).ConfigureAwait(true);
			}

			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}