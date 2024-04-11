namespace Assistant_Kira.Commands;

public abstract class Command
{
	public abstract string Name { get; }
	public abstract Task<string> ExecuteAsync(params string[] args);
}
