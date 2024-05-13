using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class VacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<VacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(VacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Current);
}