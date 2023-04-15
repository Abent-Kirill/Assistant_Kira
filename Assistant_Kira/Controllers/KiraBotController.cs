using Assistant_Kira.Models;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/message/update")]
public sealed class KiraBotController : ControllerBase
{
	private readonly BotClient _kiraBot;

    public KiraBotController(KiraBot kiraBot)
    {
		_kiraBot = kiraBot.GetBot().Result;
    }

    [HttpPost]
	public async Task<IActionResult> Update([FromBody] object update)
	{
		var udt = JsonSerializer.Deserialize<Update>(update.ToString());
		try
		{
			if (udt.Message.Text == "Привет")
			{
				await _kiraBot.SendMessageAsync(udt.Message.Chat.Id, "Hello, GameDeck!");
			}
			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}	
	}
}
