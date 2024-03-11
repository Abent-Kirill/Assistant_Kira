using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Assistant_Kira.Commands;
using Assistant_Kira.JsonConverts;
using Assistant_Kira.Models;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Assistant_Kira.Controllers;

[ApiController]
[Route("api/telegram/update")]
public sealed partial class TelegramController : ControllerBase
{
    [GeneratedRegex(@"\d\s\w\s\w")]
    private static partial Regex ConvertCurrencyRegex();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        AllowTrailingCommas = true
    };
    private readonly ILogger<TelegramController> _logger;
    private readonly IEnumerable<ICommand> _commands;
    private readonly IConfiguration _configuration;
    private readonly KiraBot _kiraBot;

    public TelegramController(ILogger<TelegramController> logger, IEnumerable<ICommand> commands, IConfiguration configuration, KiraBot kiraBot)
    {
        _logger = logger;
        _commands = commands;
        _configuration = configuration;
        _kiraBot = kiraBot;
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        _jsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
        _jsonSerializerOptions.Converters.Add(new InlineKeyboardMarkupConverter());
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] object updateObj)
    {
        Update update;
        try
        {
            ArgumentNullException.ThrowIfNull(updateObj);
            var updateDes = JsonSerializer.Deserialize<Update>(updateObj.ToString(), _jsonSerializerOptions);
            ArgumentNullException.ThrowIfNull(updateDes);
            update = updateDes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при дисириализации: {updateObj}", updateObj);
            await _kiraBot.SendTextMessageAsync(Convert.ToInt64(_configuration["BotSettings:ChatId"]), "Некорректный запрос", replyMarkup: KeyboardPatterns.Menu);
            return BadRequest(ex);
        }

        var nameCommand = string.Empty;
        var chatId = update.Message.Chat.Id;
        switch (update.Message.Type)
        {
            case MessageType.Text:
                var text = update.Message.Text;
                if (ConvertCurrencyRegex().Match(text).Success)
                {
                    nameCommand = "перевод валют";
                    break;
                }
                nameCommand = text.Split(' ')[0];
                break;
            case MessageType.Photo:
                nameCommand = "сохранить фото";
                break;
            case MessageType.Audio:
                await _kiraBot.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Video:
                await _kiraBot.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Voice:
                await _kiraBot.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardPatterns.Menu);
                break;
            case MessageType.Document:
                nameCommand = "сохранить файл";
                break;
            case MessageType.Location:
                await _kiraBot.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardPatterns.Menu);
                break;
            default:
                await _kiraBot.SendTextMessageAsync(chatId, "Данный тип команды не поддерживается", replyMarkup: KeyboardPatterns.Menu);
                break;
        }
        var currentCommand = _commands.SingleOrDefault(x => x.Name.Equals(nameCommand, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Такой команды нет");
        await currentCommand.ExecuteAsync(update, update.Message!.Text.Split(' ')[1..]);

        return Ok();
    }
}
