using Assistant_Kira.Models;
using Assistant_Kira.Repositories;

using MediatR;

namespace Assistant_Kira;

internal sealed class BackVacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<BackVacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(BackVacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Back);
}
