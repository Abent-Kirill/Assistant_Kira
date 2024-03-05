namespace Assistant_Kira.Commands;

public interface ICommand
{
	string Name { get; }
	Task<string> ExecuteAsync(IEnumerable<string> args);
}
