using System.Text;

using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class CurrencyCommand : ICommand
{
	private readonly CurrencyService _currencyService;

    public CurrencyCommand(CurrencyService currencyService)
    {
			_currencyService = currencyService;
    }

    public string Name => "курс";

	public string Execute()
	{
		var currencyExchangeList = _currencyService.GetCurrencyExchangeAsync().Result;

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
