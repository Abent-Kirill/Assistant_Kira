using Assistant_Kira.Models;
using Assistant_Kira.Repositories;
using Assistant_Kira.Requests;
using Assistant_Kira.Services;

using MediatR;

namespace Assistant_Kira.Handlers;

internal sealed class VacancyHandler(HabrCareerService habrCareerService, IRepository<Vacancy> repository) : IRequestHandler<VacancyRequest, Vacancy>
{
    public async Task<Vacancy> Handle(VacancyRequest request, CancellationToken cancellationToken)
    {
        repository.Dispose();
        repository.Contents = await habrCareerService.GetVacancies();
        return repository.Current();
    }
}
