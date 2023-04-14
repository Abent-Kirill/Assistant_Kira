using Microsoft.AspNetCore.Mvc;

using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/message/update")]
public class KiraBotController : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Update([FromBody] Update update)
	{
		if (update?.Message?.Chat == null && update?.CallbackQuery == null)
		{
			return Ok();
		}

		try
		{
			//await _commandExecutor.Execute(upd);
		}
		catch (Exception ex)
		{
			return Ok(ex.Message);
		}

		return Ok();
	}
}
