using System.Text.Json;
using System.Text.Json.Serialization;

using Assistant_Kira.JsonConverts;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/telegram/update")]
public sealed class TelegramController : ControllerBase
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        AllowTrailingCommas = true
    };
    private readonly ICommandExecutor _commandExecutor;
    private readonly ILogger<TelegramController> _logger;

    public TelegramController(ICommandExecutor commandExecutor, ILogger<TelegramController> logger)
    {
        _commandExecutor = commandExecutor;
        _logger = logger;

        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        _jsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
        _jsonSerializerOptions.Converters.Add(new InlineKeyboardMarkupConverter());
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] object updateObj)
	{
        try
		{
            ArgumentNullException.ThrowIfNull(updateObj);

            var update = JsonSerializer.Deserialize<Update>(updateObj.ToString(), _jsonSerializerOptions);

			if (update is not null)
			{
				await _commandExecutor.ExecuteAsync(update);
                return Ok();
			}
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, "Ошибка при выполнении команды");
            return BadRequest(ex);
		}
        return BadRequest("update is null");
	}
}
