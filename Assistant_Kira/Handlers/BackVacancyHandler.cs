using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class BackVacancyHandler(IRepository<Vacancy> repository) : IRequestHandler<BackVacancyRequest, Vacancy>
{
    public Task<Vacancy> Handle(BackVacancyRequest request, CancellationToken cancellationToken) => Task.Run(repository.Back);
}
