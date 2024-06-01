using Assistant_Kira.DTO;
using System.Collections.Immutable;

namespace Assistant_Kira.Repositories;

internal interface IRepository<T> : IDisposable
{
    ImmutableArray<T> Contents { get; set; }
    T Current();
    T Next();
    T Back();
}
