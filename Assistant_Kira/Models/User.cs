namespace Assistant_Kira.Models;

internal sealed class User
{
    public long Id { get; init; }
    public string FirstName { get; init; }

    public User(long id, string firstName)
    {
        Id = id;
        FirstName = firstName;
    }
}
