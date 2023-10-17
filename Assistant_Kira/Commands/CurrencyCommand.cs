using System.Text;

using Assistant_Kira.Models;
using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand : ICommand
{
	private readonly CurrencyService _currencyService;

	public string Name => "курс";

	public CurrencyCommand(CurrencyService currencyService)
	{
		_currencyService = currencyService;
	}

	public string Execute(IEnumerable<string> args)
	{
		var currencyNameArray = new List<string>() { "USD RUB", "EUR RUB", "RUB KZT" };
		if (args is not null && args.All(string.IsNullOrWhiteSpace))
		{
			currencyNameArray.AddRange(args.Where(x => !x.Equals(Name, StringComparison.OrdinalIgnoreCase)));
		}
		var currencyExchangeList = new List<Currency>();
		foreach (var currency in currencyNameArray)
		{
			var parsCurrency = currency.Split();
			currencyExchangeList.Add(_currencyService.GetCurrencyExchangeAsync(parsCurrency[0], parsCurrency[1]).Result);
		}

		var strBuilder = new StringBuilder($"Курс валюты на {DateTimeOffset.Now}\n");

		foreach (var currencyExchange in currencyExchangeList)
		{
			var RoundedRate = currencyExchange.Rates.First().Value;
			if (string.Equals(currencyExchange.Name, "KZT", StringComparison.OrdinalIgnoreCase))
			{
				strBuilder.AppendLine($"RUB = {RoundedRate} ₸");
			}
			else
			{
				strBuilder.AppendLine($"{currencyExchange.Name} = {RoundedRate} ₽");
			}
		}

		return strBuilder.ToString();
	}
}
