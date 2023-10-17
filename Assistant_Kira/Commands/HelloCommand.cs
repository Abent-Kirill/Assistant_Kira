namespace Assistant_Kira.Commands;

internal sealed class HelloCommand : ICommand
{
    public string Name => "/start";

    public string Execute(IEnumerable<string> arg) => $"Приветствую!";
}
