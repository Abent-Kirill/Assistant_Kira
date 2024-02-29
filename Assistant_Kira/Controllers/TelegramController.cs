﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/telegram/update")]
public sealed class TelegramController(ICommandExecutor commandExecutor) : ControllerBase
{
    [HttpPost]
	public async Task<IActionResult> Update([FromBody] object updateObj)
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
			if (updateObj is null)
			{
				return BadRequest("Пустой запрос");
			}

			var update = JsonSerializer.Deserialize<Update>(updateObj.ToString()!);
			if (update is not null & update!.Message is not null)
			{
				await commandExecutor.ExecuteAsync(update);
			}

			return Ok();
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
}
