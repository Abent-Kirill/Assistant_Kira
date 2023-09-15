using Assistant_Kira.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/message/update")]
public sealed class KiraBotController : ControllerBase
{
	private readonly KiraBot _kira;
	public KiraBotController(KiraBot kira) => _kira = kira;

	[HttpPost]
	public async Task<IActionResult> Update([FromBody] object updateObj)
	{
		try
		{
			var update = JsonConvert.DeserializeObject<Update>(updateObj.ToString()!);
			if (update is not null & update!.Message is not null)
			{
				var telegramCommandExecutor = new CommandExecutor(_kira.TelegramApi);
				await telegramCommandExecutor.ExecuteAsync(update).ConfigureAwait(true);
			}

			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}