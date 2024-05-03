namespace Assistant_Kira.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    T Next();
    T Back();
}
