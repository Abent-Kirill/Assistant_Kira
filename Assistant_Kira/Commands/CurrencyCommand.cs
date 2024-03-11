using System.Collections.Immutable;
using System.Text;

using Assistant_Kira.Models;
using Assistant_Kira.Models.Currencys;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand(ApilayerCurrencyService currencyService, KiraBot kiraBot, ILogger<CurrencyCommand> logger) : ICommand
{
    public string Name => "курс";

    private readonly Dictionary<string, string> _currentCurrencyNameArray = new()
    {
            {"USD", "RUB"},
            {"EUR", "RUB"},
            {"RUB", "KZT"}
    };

    public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        if (args is not null && args.All(string.IsNullOrWhiteSpace))
        {
            var filtringArgs = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase)).ToImmutableArray();
            if (filtringArgs.Any())
            {
                _currentCurrencyNameArray.Add(filtringArgs[0], filtringArgs[1]);
            }
        }

        var currencyList = new List<Currency>();
        try
        {
            foreach (var currency in _currentCurrencyNameArray)
            {
                currencyList.Add(await currencyService.GetCurrencyExchangeAsync(currency.Key, currency.Value));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, ex.Message, replyMarkup: KeyboardPatterns.Menu);
            return;
        }
        var strBuilder = new StringBuilder($"Курс валюты на {DateTimeOffset.Now}\n");

        foreach (var currencyExchange in currencyList)
        {
            var roundedRate = currencyExchange.Rates.First().Value;
            if (string.Equals(currencyExchange.Name, "RUB", StringComparison.OrdinalIgnoreCase))
            {
                strBuilder.AppendLine($"RUB = {roundedRate} ₸");
                continue;
            }
            strBuilder.AppendLine($"{currencyExchange.Name} = {roundedRate} ₽");
        }

        await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, strBuilder.ToString(), replyMarkup: KeyboardPatterns.Menu);
    }
}
