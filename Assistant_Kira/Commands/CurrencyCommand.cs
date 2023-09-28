using System.Text;
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

	public string Execute()
	{
		var v = new string[]{"KZT", "USD" };
		var currencyExchangeList = _currencyService.GetCurrencyExchangeAsync().Result;

		var strBuilder = new StringBuilder($"Курс валюты на {DateTimeOffset.Now}\n");

		foreach (var currencyExchange in currencyExchangeList)
		{
			strBuilder.AppendLine($"{currencyExchange.Name} = {currencyExchange.Rates.First().Value} ₽");
		}

		return strBuilder.ToString();
	}
}
