namespace Assistant_Kira.Commands;

internal sealed class HelloCommand : Command
{
    public override string Name => "/start";

    public override async Task<string> ExecuteAsync(params string[] arg) => await Task.Run(() => $"{GetGreeting()}\nЧто хотите сделать?");
    private static string GetGreeting()
    {
        var currentTime = DateTime.Now;
        var currentHour = currentTime.Hour;

        if (currentHour >= 6 && currentHour < 12)
        {
            return "Доброе утро";
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            return "Добрый день";
        }
        else
        {
            return "Добрый вечер";
        }
    }
}
