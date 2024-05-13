namespace Assistant_Kira.Repositories;

public interface IRepository<T> : IDisposable
{
    T Current();
    T Next();
    T Back();
}
