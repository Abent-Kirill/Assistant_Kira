namespace Assistant_Kira.Commands;

internal sealed class HelloCommand : ICommand
{
    public string Name => "/start";

    public Task<string> ExecuteAsync(IEnumerable<string> arg) => Task.FromResult("Приветствую!");
}
