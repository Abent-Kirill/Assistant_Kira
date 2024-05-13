using System.Text;

using Assistant_Kira.Requests;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Assistant_Kira.Controllers;

/// <summary>
/// Для тестирования в режиме чата
/// </summary>
/// <param name="mediator"></param>
[Route("api/Chat/[controller]")]
[Produces("application/json")]
[ApiController]
public sealed class ChatController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Симуляция чата, только без update
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string text)
    {
        if (text.IsMatchConvertCurrencyRegex())
        {
            var textArray = text.Split(' '); //TODO: Сделать определение через regex
            var result = await mediator.Send(new ConvertCurrencyRequest(Convert.ToUInt32(textArray[0]), textArray[1], textArray[2]));
            return Ok(result);
        }

        if (text.IsMatchCalendarEventRegex())
        {
            var result = await mediator.Send(new CreateCalendarEventRequest(text));
            return Ok(result ? "Успех" : "Bad");
        }

        switch (text.ToLower())
        {
            case "погода":
                var weather = await mediator.Send(new WeatherRequest("Samara"));
                return Ok(weather);
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
                return Ok(strBuilder.ToString());
            case "новости":
                var news = await mediator.Send(new NewsRequest());
                return Ok(news);
            case "вакансии":
                var vacancy = await mediator.Send(new VacancyRequest());
                return Ok(vacancy);
            default: return BadRequest();
        }
    }
}
