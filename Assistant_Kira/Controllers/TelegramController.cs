using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Assistant_Kira.JsonConverts;
using Assistant_Kira.Requests;
using Assistant_Kira.Services;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Assistant_Kira.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/telegram/update")]
public sealed partial class TelegramController(IMediator mediator, IConfiguration configuration, ITelegramBotClient botClient, ServerService serverService) : ControllerBase
{
    //TODO: Написать unit-tests
    [GeneratedRegex(@"^\d+\s\w{3}\s\w{3}$")]
    private static partial Regex ConvertCurrencyRegex();
    [GeneratedRegex(@"(?<hour>\d{1,2}):(?<minute>\d{1,2})", RegexOptions.IgnoreCase, "ru-KZ")]
    private static partial Regex CalendarEventRegex();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] object updateObj)
    {
        var jsonOpt = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        jsonOpt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        jsonOpt.Converters.Add(new UnixTimestampConverter());
        jsonOpt.Converters.Add(new InlineKeyboardMarkupConverter());
        
        ArgumentNullException.ThrowIfNull(updateObj);
        Update update = JsonSerializer.Deserialize<Update>(updateObj.ToString(), jsonOpt);
        long chatId;
        Message message;

        if (update.CallbackQuery != null)
        {
            message = update.CallbackQuery.Message;
            chatId = message.Chat.Id;
            
            switch (update.CallbackQuery.Data)
            {
                case "next_news":
                    var nextNews = await mediator.Send(new NextNewsRequest());
                    await botClient.EditMessageTextAsync(chatId, message.MessageId, nextNews.ToString(), replyMarkup: KeyboardSamples.NewsKeyboard);
                    return Ok();
                case "back_news":
                    var backNews = await mediator.Send(new BackNewsRequest());
                    await botClient.EditMessageTextAsync(chatId, message.MessageId, backNews.ToString(), replyMarkup: KeyboardSamples.NewsKeyboard);
                    return Ok();
                case "next_vacancy": return Ok();
                case "back_vacancy": return Ok();
            }
        }
        else
        {
            chatId = update.Message.Chat.Id;
            message = update.Message;
        }

        if (chatId != Convert.ToInt64(configuration["BotSettings:ChatId"]))
        {
            await botClient.SendTextMessageAsync(chatId, "Вы не являетесь человеком с которым я работаю. Всего хорошего");
            return BadRequest();
        }

        switch (message.Type)
        {
            case MessageType.Text:
                var text = message.Text;
                if (ConvertCurrencyRegex().Match(text).Success)
                {
                    var textArray = text.Split(' '); //TODO: Сделать определение через regex
                    var result = await mediator.Send(new ConvertCurrencyRequest(Convert.ToUInt32(textArray[0]), textArray[1], textArray[2]));
                    await botClient.SendTextMessageAsync(chatId, result.ToString(), replyMarkup: KeyboardSamples.Menu);
                    return Ok();
                }

                if (CalendarEventRegex().Match(text).Success)
                {
                    var result = await mediator.Send(new CreateCalendarEventRequest(text));
                    await botClient.SendTextMessageAsync(chatId, result ? "Успех" : "Bad", replyMarkup: KeyboardSamples.Menu);
                    return Ok();
                }

                switch (text.ToLower())
                {
                    case "погода":
                        var weather = await mediator.Send(new WeatherRequest("Samara"));
                        await botClient.SendTextMessageAsync(chatId, weather.ToString(), replyMarkup: KeyboardSamples.Menu);
                        return Ok();
                    case "курс":
                        var currences = await mediator.Send(new GetCurrencyRequest());
                        var strBuilder = new StringBuilder($"Курс валюты на {DateTimeOffset.Now}\n");

                        foreach (var currencyExchange in currences)
                        {
                            var roundedRate = currencyExchange.Rates.First().Value;
                            if (string.Equals(currencyExchange.Name, "RUB", StringComparison.OrdinalIgnoreCase))
                            {
                                strBuilder.AppendLine($"RUB = {roundedRate} ₸");
                                continue;
                            }
                            strBuilder.AppendLine($"{currencyExchange.Name} = {roundedRate} ₽");
                        }
                        await botClient.SendTextMessageAsync(chatId, strBuilder.ToString(), replyMarkup: KeyboardSamples.Menu);
                        return Ok();
                    case "новости":
                        var news = await mediator.Send(new NewsRequest());
                        await botClient.SendTextMessageAsync(chatId, news.ToString(), replyMarkup: KeyboardSamples.NewsKeyboard);
                        return Ok();
                }
                break;
            case MessageType.Photo:
                var photos = message.Photo![^1];
                ArgumentNullException.ThrowIfNull(photos, nameof(photos));
                await serverService.CopyToServer(photos, configuration["Paths:Photos"]);
                break;
            case MessageType.Audio:
                await botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Video:
                await botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Voice:
                await botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            case MessageType.Document:
                var document = message.Document;
                ArgumentNullException.ThrowIfNull(document, nameof(document));
                await serverService.CopyToServer(document, configuration["Paths:Files"]!);
                break;
            case MessageType.Location:
                await botClient.SendTextMessageAsync(chatId, "Это действие ещё  не реализовано", replyMarkup: KeyboardSamples.Menu);
                break;
            default:
                await botClient.SendTextMessageAsync(chatId, "Данный тип команды не поддерживается", replyMarkup: KeyboardSamples.Menu);
                break;
        }
        return Ok();
    }
}
