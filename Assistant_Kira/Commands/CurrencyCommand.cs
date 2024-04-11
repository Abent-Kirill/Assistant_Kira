using System.Collections.Immutable;
using System.Text;

using Assistant_Kira.Models.Currencys;
using Assistant_Kira.Services.CurrencyServices;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand(ICurrencyService currencyService, ILogger<CurrencyCommand> logger) : Command
{
    public override string Name => "курс";

    private readonly Dictionary<string, string> _currentCurrencyNameArray = new()
    {
            {"USD", "RUB"},
            {"EUR", "RUB"},
            {"RUB", "KZT"}
    };

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        if (args is not null && args.All(string.IsNullOrWhiteSpace))
        {
            var filtringArgs = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase)).ToImmutableArray();
            if (filtringArgs.Any())
            {
                _currentCurrencyNameArray.Add(filtringArgs[0], filtringArgs[1]);
            }
        }

        var currencyList = ImmutableList.CreateBuilder<Currency>().ToImmutable();
        try
        {
            foreach (var currency in _currentCurrencyNameArray)
            {
                currencyList.Add(await currencyService.GetCurrencyExchangeAsync(currency.Key, currency.Value));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Входные данные: {args}", args);
            return ex.Message;
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

        return strBuilder.ToString();
    }
}
