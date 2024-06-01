using System.Text;

using Assistant_Kira.Requests;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Assistant_Kira.Controllers;

/// <summary>
/// Для тестирования отдельного функционала
/// </summary>
[Route("api/Testing/[controller]")]
[Produces("application/json")]
[ApiController]
public sealed class TestController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Конвертация валют из текста
    /// </summary>
    /// <param name="text">строка в формате число код_валюты_из код_волюты_в_которую</param>
    /// <returns>Переведённую валюту</returns>
    [HttpGet("currency/convert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertCurrency(string text)
    {
        if (text.IsMatchConvertCurrencyRegex())
        {
            var textArray = text.Split(' '); //TODO: Сделать определение через regex
            var result = await mediator.Send(new ConvertCurrencyRequest(Convert.ToUInt32(textArray[0]), textArray[1], textArray[2]));
            return Ok(result);
        }
        return BadRequest("Не удалось перевести или ошибка в тексте");
    }
    /// <summary>
    /// Создание события в календаре
    /// </summary>
    /// <param name="text">Текст в формате Что за событие время</param>
    /// <returns></returns>
    [HttpPut("calendar/event/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalendarEvent(string text)
    {
        if (text.IsMatchCalendarEventRegex())
        {
            _ = await mediator.Send(new CreateCalendarEventRequest(text));
            return Created();
        }
        return BadRequest("Ошибка в тексте");
    }

    /// <summary>
    /// Показ погоды
    /// </summary>
    /// <param name="city">Город</param>
    /// <returns>Актуальную информацию о погоде</returns>
    [HttpGet("weather")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Weather(string city = "Samara")
    {
        var weather = await mediator.Send(new WeatherRequest(city));
        return Ok(weather);
    }

    /// <summary>
    /// Курс валют
    /// </summary>
    /// <returns>Список валюты и сколько стоит</returns>
    [HttpGet("currency/exch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Currency()
    {
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
        return Ok(strBuilder.ToString());
    }

    /// <summary>
    /// Показ новостей
    /// </summary>
    /// <returns>Выводит последнюю новость</returns>
    [HttpGet("news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> News()
    {
        var news = await mediator.Send(new NewsRequest(""));
        return Ok(news);
    }
}
