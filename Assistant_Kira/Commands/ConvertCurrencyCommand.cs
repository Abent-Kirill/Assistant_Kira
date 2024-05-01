using Assistant_Kira.Services.CurrencyServices;

namespace Assistant_Kira.Commands;

internal sealed class ConvertCurrencyCommand(ICurrencyService currencyService, ILogger<ConvertCurrencyCommand> logger) : Command
{
    public override string Name => "перевод валют";

    public override async Task<string> ExecuteAsync(params string[] args)
    {
        string text;
        try
        {
            var data = args.ToList();
            var currencyData = await currencyService.CurrencyConversionAsync(
                Convert.ToUInt32(data[0]), data[1], data[2]);

            text = currencyData.ToString();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Входные данные: {args}", args);
            text = ex.Message;
        }
        return text;
    }
}
