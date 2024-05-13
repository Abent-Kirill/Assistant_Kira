using Assistant_Kira.Models;
using Assistant_Kira.Repositories;

using MediatR;

namespace Assistant_Kira;

internal sealed class VacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<VacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(VacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Current);
}