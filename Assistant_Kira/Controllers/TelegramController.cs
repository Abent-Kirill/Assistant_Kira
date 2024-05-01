using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Assistant_Kira.Commands;
using Assistant_Kira.JsonConverts;
using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Assistant_Kira.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/telegram/update")]
public sealed partial class TelegramController : ControllerBase
{
    //TODO: Написать unit-tests
    [GeneratedRegex(@"^\d+\s\w{3}\s\w{3}$")]
    private static partial Regex ConvertCurrencyRegex();
    [GeneratedRegex(@"(?<hour>\d{1,2}):(?<minute>\d{1,2})", RegexOptions.IgnoreCase, "ru-KZ")]
    private static partial Regex CalendarEventRegex();

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
    private readonly IReadOnlyCollection<Command> _commands;
    private readonly IConfiguration _configuration;
    private readonly ITelegramBotClient _botClient;
    private readonly ServerService _serverService;

    public TelegramController(ILogger<TelegramController> logger, IEnumerable<Command> commands, IConfiguration configuration, ITelegramBotClient botClient, ServerService serverService)
    {
        _logger = logger;
        _commands = new ReadOnlyCollection<Command>(commands.ToList());
        _configuration = configuration;
        _botClient = botClient;
        _serverService = serverService;
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        _jsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
        _jsonSerializerOptions.Converters.Add(new InlineKeyboardMarkupConverter());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            await _botClient.SendTextMessageAsync(Convert.ToInt64(_configuration["BotSettings:ChatId"]), "Некорректный запрос", replyMarkup: KeyboardSamples.Menu);
            return BadRequest(ex);
        }
        var nameCommand = string.Empty;
        if(update.CallbackQuery != null)
        {
            var chatIdCallBack = update.CallbackQuery.Message.Chat.Id;
            if (chatIdCallBack != Convert.ToInt64(_configuration["BotSettings:ChatId"]))
            {
                await _botClient.SendTextMessageAsync(chatIdCallBack, "Вы не являетесь человеком с которым я работаю. Всего хорошего");
                return BadRequest();
            }
            if (update.CallbackQuery.Data.Equals("Вперёд", StringComparison.CurrentCultureIgnoreCase))
            {
                Command currentCommand = _commands.Single(x => x.Name.Equals("Новости", StringComparison.OrdinalIgnoreCase));
                var result = await currentCommand.ExecuteAsync("Вперёд");
                await _botClient.EditMessageTextAsync(chatIdCallBack, update.CallbackQuery.Message.MessageId, result, replyMarkup: KeyboardSamples.NewsKeyboard);
            }
            if (update.CallbackQuery.Data.Equals("Назад", StringComparison.CurrentCultureIgnoreCase))
            {
                Command currentCommand = _commands.Single(x => x.Name.Equals("Новости", StringComparison.OrdinalIgnoreCase));
                var result = await currentCommand.ExecuteAsync("Назад");
                await _botClient.EditMessageTextAsync(chatIdCallBack, update.CallbackQuery.Message.MessageId, result, replyMarkup: KeyboardSamples.NewsKeyboard);
            }
            if (update.CallbackQuery.Data.Equals("Вперёд v", StringComparison.CurrentCultureIgnoreCase))
            {
                Command currentCommand = _commands.Single(x => x.Name.Equals("Вакансии", StringComparison.OrdinalIgnoreCase));
                var result = await currentCommand.ExecuteAsync("Вперёд");
                await _botClient.EditMessageTextAsync(chatIdCallBack, update.CallbackQuery.Message.MessageId, result, replyMarkup: KeyboardSamples.VacanciesKeyboard);
            }
            if (update.CallbackQuery.Data.Equals("Назад v", StringComparison.CurrentCultureIgnoreCase))
            {
                Command currentCommand = _commands.Single(x => x.Name.Equals("Вакансии", StringComparison.OrdinalIgnoreCase));
                var result = await currentCommand.ExecuteAsync("Назад");
                await _botClient.EditMessageTextAsync(chatIdCallBack, update.CallbackQuery.Message.MessageId, result, replyMarkup: KeyboardSamples.VacanciesKeyboard);
            }
            return Ok();
        }
        var chatId = update.Message.Chat.Id;
        if (chatId != Convert.ToInt64(_configuration["BotSettings:ChatId"]))
        {
            await _botClient.SendTextMessageAsync(chatId, "Вы не являетесь человеком с которым я работаю. Всего хорошего");
            return BadRequest();
        }
        switch (update.Message.Type)
        {
            case MessageType.Text:
                var text = update.Message.Text;
                if (ConvertCurrencyRegex().Match(text).Success)
                {
                    var currentCommand = _commands.SingleOrDefault(x => x.Name.Equals("перевод валют", StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException("Такой команды нет");
                    string result = await currentCommand.ExecuteAsync(update.Message!.Text.Split(' '));
                    await _botClient.SendTextMessageAsync(chatId, result, replyMarkup: KeyboardSamples.Menu);
                    return Ok();
                }

                if (CalendarEventRegex().Match(text).Success)
                {
                        var currentCommand = _commands.SingleOrDefault(x => x.Name.Equals("Новое событие", StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException("Такой команды нет");
                        string result = await currentCommand.ExecuteAsync(text.Split(' '));
                    await _botClient.SendTextMessageAsync(chatId, result, replyMarkup: KeyboardSamples.Menu);
                        return Ok();
                }
                nameCommand = text.Split(' ')[0];
                break;
            case MessageType.Photo:
                var photos = update.Message.Photo![^1];
                ArgumentNullException.ThrowIfNull(photos, nameof(photos));
                await _serverService.CopyToServer(photos, _configuration["Paths:Photos"]);
                break;
            case MessageType.Audio:
                await _botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Video:
                await _botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Voice:
                await _botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Document:
                try
                {
                    var document = update.Message.Document;
                    ArgumentNullException.ThrowIfNull(document, nameof(document));
                    await _serverService.CopyToServer(document, _configuration["Paths:Files"]!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Входные данные: {args}", update.Message.Document);
                }
                break;
            case MessageType.Location:
                await _botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            default:
                await _botClient.SendTextMessageAsync(chatId, "Данный тип команды не поддерживается", replyMarkup: KeyboardSamples.Menu);
                break;
        }
        try
        {
            var currentCommand = _commands.SingleOrDefault(x => x.Name.Equals(nameCommand, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Такой команды нет");
            string result = await currentCommand.ExecuteAsync(update.Message!.Text.Split(' ')[1..]);
            IReplyMarkup keyboard = KeyboardSamples.Menu;
            if (currentCommand.GetType() == typeof(NewsCommand))
            {
                keyboard = KeyboardSamples.NewsKeyboard;
            }
            if (currentCommand.GetType() == typeof(HabrVacanciesCommand))
            {
                keyboard = KeyboardSamples.VacanciesKeyboard;
            }
            await _botClient.SendTextMessageAsync(chatId, result, replyMarkup: keyboard);
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex, "Ошибка при поиске команды: {nameCommand}", nameCommand);
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, ex.Message, replyMarkup: KeyboardSamples.Menu);
        }

        return Ok();
    }
}
