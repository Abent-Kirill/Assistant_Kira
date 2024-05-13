using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class NextVacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<NextVacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(NextVacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Next);
}
