using Assistant_Kira.Models;
using Assistant_Kira.Repositories;

using MediatR;

namespace Assistant_Kira;

internal sealed class NextVacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<NextVacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(NextVacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Next);
}
