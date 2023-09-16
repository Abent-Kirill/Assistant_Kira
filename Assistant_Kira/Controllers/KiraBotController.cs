using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
		try
		{
			var update = JsonConvert.DeserializeObject<Update>(updateObj.ToString()!);
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