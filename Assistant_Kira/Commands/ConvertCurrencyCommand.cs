using Assistant_Kira.Services;

namespace Assistant_Kira.Commands;

internal sealed class ConvertCurrencyCommand(CurrencyService currencyService) : ICommand
{
    public string Name => "перевод валют";

    public string Execute(IEnumerable<string> args)
    {
        var data = args.ToList();
        var currencyData = currencyService.CurrencyConversionAsync(Convert.ToInt32(data[0]), char.Parse(data[1]), char.Parse(data[2])).Result;

        return currencyData.ToString();
    }
}
