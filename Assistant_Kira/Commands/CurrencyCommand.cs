using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

using Assistant_Kira.DTO.Currencys;
using Assistant_Kira.Services.CurrencyServices;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand(ICurrencyService currencyService, ILogger<CurrencyCommand> logger) : Command
{
    public override string Name => "курс";

    private readonly ReadOnlyDictionary<string, string> _currentCurrencyNameArray = new(new Dictionary<string, string>()
    {
            {"USD", "RUB"},
            {"EUR", "RUB"},
            {"RUB", "KZT"}
    });
    
    public override async Task<string> ExecuteAsync(params string[] args)
    {
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
