using System.Collections.Immutable;
using System.Text;

using Assistant_Kira.Models.Currencys;
using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand(CurrencyService currencyService) : ICommand
{
    public string Name => "курс";

    private readonly Dictionary<string, string> _currencyNameArray = new()
    {
            { "USD", "RUB" },
            {"EUR", "RUB"},
            {"RUB", "KZT" }
    };

    public async Task<string> ExecuteAsync(IEnumerable<string> args)
	{
		if (args is not null && args.All(string.IsNullOrWhiteSpace))
		{
			var filtringArgs = args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase)).ToImmutableArray();
            if(filtringArgs.Any())
            {
                _currencyNameArray.Add(filtringArgs[0], filtringArgs[1]);
            }
        }

		var currencyList = new List<Currency>();
		foreach (var currency in _currencyNameArray)
		{
			currencyList.Add(await currencyService.GetCurrencyExchangeAsync(currency.Key, currency.Value));
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
