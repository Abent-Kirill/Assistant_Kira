﻿using Assistant_Kira.Models;

using MediatR;

namespace Assistant_Kira;

internal sealed record BackNewsRequest : IRequest<NewsContent>
{
}