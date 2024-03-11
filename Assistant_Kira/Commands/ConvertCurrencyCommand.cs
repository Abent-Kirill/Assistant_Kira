using Assistant_Kira.Models;
using Assistant_Kira.Services;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Assistant_Kira.Commands;

internal sealed class ConvertCurrencyCommand(ApilayerCurrencyService currencyService, KiraBot kiraBot, ILogger<ConvertCurrencyCommand> logger) : ICommand
{
    public string Name => "перевод валют";

    public async Task ExecuteAsync(Update update, IEnumerable<string>? args = null)
    {
        string text;
        try
        {
            var data = args.ToList();
            var currencyData = await currencyService.CurrencyConversionAsync(Convert.ToInt32(data[0]), char.Parse(data[1]), char.Parse(data[2]));

            text = currencyData.ToString();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Входные данные: {update}, {args}", update, args);
            text = ex.Message;
        }
        await kiraBot.SendTextMessageAsync(update.Message.Chat.Id, text, replyMarkup: KeyboardPatterns.Menu);
    }
}
