namespace Assistant_Kira.Commands;

public interface ICommand
{
	string Name { get; }
	string Execute(IEnumerable<string> args);
}
