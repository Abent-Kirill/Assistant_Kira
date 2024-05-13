﻿using Assistant_Kira.Models;

using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record VacancyRequest : IRequest<Vacancy>
{
}